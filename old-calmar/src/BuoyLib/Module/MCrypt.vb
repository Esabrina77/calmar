Imports System
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text

Public Module MCrypt

    'Doit correspondre à 64 bits, 8 octets.
    Private Const sSecretKey As String = "Galinete"

    Public Function AES_Encrypt(ByVal input As String, Optional ByVal sKey As String = sSecretKey) As String
        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim encrypted As String = ""
        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))

            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = Security.Cryptography.CipherMode.ECB
            Dim DESEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(input)
            encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Return encrypted

        Catch ex As Exception
            Return ""
        End Try

    End Function

    Public Function AES_Decrypt(ByVal input As String, Optional ByVal sKey As String = sSecretKey) As String

        Dim AES As New System.Security.Cryptography.RijndaelManaged
        Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim decrypted As String = ""

        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))

            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = Security.Cryptography.CipherMode.ECB
            Dim DESDecrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = Convert.FromBase64String(input)
            decrypted = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))
            Return decrypted
        Catch ex As Exception
            Return ""
        End Try

    End Function

    Public Sub EncryptFile(ByVal sInputFilename As String,
                   ByVal sOutputFilename As String,
                   Optional ByVal sKey As String = sSecretKey)

        Dim fsInput As New FileStream(sInputFilename,
                                    FileMode.Open, FileAccess.Read)
        Dim fsEncrypted As New FileStream(sOutputFilename,
                                    FileMode.Create, FileAccess.Write)

        Dim DES As New DESCryptoServiceProvider()

        'Définit la clé secrète pour l'algorithme DES.
        'Une clé de 64 bits et un vecteur d'initialisation sont requis pour ce fournisseur
        DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey)

        'Définit le vecteur d'initialisation.
        DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey)

        'crée un crypteur DES à partir de cette instance
        Dim desencrypt As ICryptoTransform = DES.CreateEncryptor()
        'Crée un flux de cryptage qui transforme le flux de fichier à l'aide du cryptage DES
        Dim cryptostream As New CryptoStream(fsEncrypted,
                                            desencrypt,
                                            CryptoStreamMode.Write)

        'Lit le texte du fichier dans le tableau d'octets
        Dim bytearrayinput(fsInput.Length - 1) As Byte
        fsInput.Read(bytearrayinput, 0, bytearrayinput.Length)
        'écrit le fichier crypté à l'aide de DES
        cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length)
        cryptostream.Close()
    End Sub

    Public Sub DecryptFile(ByVal sInputFilename As String,
        ByVal sOutputFilename As String,
        Optional ByVal sKey As String = sSecretKey)

        Dim DES As New DESCryptoServiceProvider()
        'Une clé de 64 bits et un vecteur d'initialisation sont requis pour ce fournisseur.
        'Définit la clé secrète pour l'algorithme DES.
        DES.Key() = ASCIIEncoding.ASCII.GetBytes(sKey)
        'Définit le vecteur d'initialisation.
        DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey)

        'crée un flux de fichier pour lire le fichier crypté de retour
        Dim fsread As New FileStream(sInputFilename, FileMode.Open, FileAccess.Read)
        'crée un décrypteur DES à partir de l'instance DES
        Dim desdecrypt As ICryptoTransform = DES.CreateDecryptor()
        'crée un flux de cryptage, défini pour lire et effectuer une transformation
        'de décryptage DES sur les octets entrants
        Dim cryptostreamDecr As New CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read)
        'imprime le contenu du fichier décrypté
        Dim fsDecrypted As New StreamWriter(sOutputFilename)
        fsDecrypted.Write(New StreamReader(cryptostreamDecr).ReadToEnd)
        fsDecrypted.Flush()
        fsDecrypted.Close()
    End Sub

End Module