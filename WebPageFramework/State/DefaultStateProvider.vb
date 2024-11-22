Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography
Imports System.Text

Public Class DefaultStateProvider
    Implements IStateProvider

    Private ReadOnly secretPassword As String
    Private ReadOnly secretSalt As String

    ' Требуется ли шифрование данных состояния представления
    Private ReadOnly isEncrypting As String

    'Требуется ли сжатие данных состояния представления
    Private ReadOnly isCompressing As String

    Public Sub New(SecretPassword As String, SecretSalt As String, IsEncrypting As Boolean, IsCompressing As Boolean)
        Me.secretPassword = SecretPassword
        Me.secretSalt = SecretSalt
        Me.isEncrypting = IsEncrypting
        Me.isCompressing = IsCompressing
    End Sub

    ''' <summary>
    ''' Сохранить состояние в сериализованное представление
    ''' </summary>
    Public Function SaveState(State As StateObject) As String Implements IStateProvider.SaveState
        Try
            ' Сериализуем
            Dim bytesState() As Byte = Encoding.UTF8.GetBytes(State.SerializeWithTypeInfo())

            ' Сжимаем
            If isCompressing Then
                bytesState = GZipPack(bytesState)
            End If

            ' Шифруем
            If isEncrypting Then
                bytesState = AES256Encode(bytesState, secretPassword, secretSalt)
            End If

            ' Пакуем
            Return Convert.ToBase64String(bytesState)

        Catch ex As Exception
            Throw New StateWrongDataException("Невозможно сформировать объект состояния", ex)

        End Try
    End Function

    ''' Восстановить состояние из сериализованного представления
    Public Function LoadState(PackedState As String) As StateObject Implements IStateProvider.LoadState
        Try
            ' Распаковываем
            Dim bytesState() As Byte = Convert.FromBase64String(PackedState)

            ' Расшифруем
            If isEncrypting Then
                bytesState = AES256Decode(bytesState, secretPassword, secretSalt)
            End If

            ' Расжимаем
            If isCompressing Then
                bytesState = GZipUnpack(bytesState)
            End If

            ' Десериализуем
            Return Encoding.UTF8.GetString(bytesState).DeserializeWithTypeInfo()

        Catch ex As Exception
            Throw New StateWrongDataException("Невозможно сформировать объект состояния", ex)

        End Try
    End Function

    ''' <summary>
    ''' Упаковать данные в GZIP
    ''' </summary>
    Private Shared Function GZipPack(Source() As Byte) As Byte()
        Using compressedStream As New MemoryStream()
            Using gzipStream As New GZipStream(compressedStream, CompressionMode.Compress, True)
                gzipStream.Write(Source, 0, Source.Length)
            End Using

            Return compressedStream.ToArray()
        End Using
    End Function

    ''' <summary>
    ''' Распаковать данные из GZIP
    ''' </summary>
    Private Shared Function GZipUnpack(Source() As Byte) As Byte()
        Using compressedStream As New MemoryStream(Source)
            Using gzipStream As New GZipStream(compressedStream, CompressionMode.Decompress)
                Using decompressedStream As New MemoryStream()
                    gzipStream.CopyTo(decompressedStream)
                    Return decompressedStream.ToArray()
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Создание ключа по пользовательскому паролю и соли. Выполняется деривация ключа с использованием PBKDF2. Для AES256 нужен ключ 32 бита
    ''' </summary>
    Private Shared Function GenerateAESKey(Password As String, Salt As String, Optional KeySizeBits As Integer = 256) As Byte()
        Dim pbkdf2 As New Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(Password), Encoding.UTF8.GetBytes(Salt), 10000, HashAlgorithmName.SHA256)
        Return pbkdf2.GetBytes(CInt(KeySizeBits / 8))
    End Function

    ''' <summary>
    ''' Зашифровать данные в AES
    ''' </summary>
    Private Shared Function AES256Encode(Source() As Byte, Password As String, Salt As String) As Byte()
        Using aesAlg As Aes = Aes.Create()
            aesAlg.IV = RandomNumberGenerator.GetBytes(16)
            aesAlg.Key = GenerateAESKey(Password, Salt)
            aesAlg.Mode = CipherMode.CBC
            aesAlg.Padding = PaddingMode.Zeros

            Dim encryptor = aesAlg.CreateEncryptor()

            Using msEncrypt As New MemoryStream()
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length) ' Запишем сначала IV

                Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write) ' Далее пишем шифротекст
                    csEncrypt.Write(Source, 0, Source.Length)
                End Using

                Return msEncrypt.ToArray()
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Расшифровать данные в AES
    ''' </summary>
    Private Shared Function AES256Decode(Source() As Byte, Password As String, Salt As String) As Byte()
        Using aesAlg As Aes = Aes.Create()
            aesAlg.IV = Source.Take(16).ToArray() ' Читаем IV
            aesAlg.Key = GenerateAESKey(Password, Salt)
            aesAlg.Mode = CipherMode.CBC
            aesAlg.Padding = PaddingMode.Zeros

            Dim decryptor = aesAlg.CreateDecryptor()

            Using msDecrypt As New MemoryStream(Source, aesAlg.IV.Length, Source.Length - aesAlg.IV.Length)
                Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)

                    Dim decryptedBytes(Source.Length) As Byte
                    Dim inRead = csDecrypt.Read(decryptedBytes, 0, decryptedBytes.Length)

                    Return decryptedBytes.Take(inRead).ToArray()
                End Using
            End Using
        End Using
    End Function

End Class
