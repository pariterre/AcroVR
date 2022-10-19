using System;
using System.IO;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

// =================================================================================================================================================================
/// <summary> Lecture de quelques informations système de l'ordinateur, qui sera utilisé pour limiter l'utilisation du logiciel AcroVR à certains ordinateurs seulement. </summary>

public class GetAccessKey : MonoBehaviour
{
	public static GetAccessKey Instance;

	public InputField inputFieldExpirationDate;
	public InputField inputFieldRequestNumber;
    public Text textActivationKey;

	static string refChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	static int[] refEncryptionNumbers = new int[20] { 5, 9, 15, 7, 1, 3, 12, 25, 25, 2, 17, 8, 0, 23, 4, 23, 19, 6, 20, 13 };

	// =================================================================================================================================================================
	/// <summary> Initialisation du script. </summary>

	void Start()
    {
		Instance = this;
	}

	// =================================================================================================================================================================
	/// <summary> Le bouton Générer clé a été appuyer. </summary>

	public void ButtonCreateKey()
	{
		string expirationDate = inputFieldExpirationDate.text;
		string requestNumber = inputFieldRequestNumber.text;

        string activationKey = string.Format("Clé d'activation = {0}", EncryptedAccessKey(expirationDate, requestNumber));
        textActivationKey.text = activationKey;

		//Application.Quit();
	}

    // =================================================================================================================================================================
    /// <summary> Le bouton Terminer a été appuyer. </summary>

    public void ButtonQuit()
    {
        Application.Quit();
    }

    // =================================================================================================================================================================
    /// <summary> Encrypter l'information pour identifier l'ordinateur. </summary>

    public static string EncryptedComputerInfos(string computerInfos)
	{
		int sum = 0;
		for (int i = 0; i < computerInfos.Length; i++)
			sum += (255 - computerInfos[i]) % 100;
		int sum2 = (sum + 3) * 3 + 358743;
		return sum.ToString() + sum2.ToString();
	}

	// =================================================================================================================================================================
	/// <summary> Encrypter la clé d'activation. </summary>

	public static string EncryptedAccessKey(string expirationDate, string requestNumber)
	{
		// Aucun commentaire n'a été écrit ici volontairement

		int[] intExpirationDate = ConvertToInteger(expirationDate);
		int[] intRequestNumber = ConvertToInteger(requestNumber);

		int[] intRequestNumberLen;
		if (requestNumber.Length < 10)
			intRequestNumberLen = ConvertToInteger("0" + requestNumber.Length.ToString());
		else
			intRequestNumberLen = ConvertToInteger(requestNumber.Length.ToString());

		int checkSumNumber = 0;
		for (int i = 0; i < intExpirationDate.Length; i++)
			checkSumNumber += intExpirationDate[i];
		for (int i = 0; i < intRequestNumber.Length; i++)
			checkSumNumber += intRequestNumber[i];
		checkSumNumber = checkSumNumber % 100;
		int[] intCheckSum;
		if (checkSumNumber < 10)
			intCheckSum = ConvertToInteger("0" + checkSumNumber.ToString());
		else
			intCheckSum = ConvertToInteger(checkSumNumber.ToString());

		int iRefN = 0;
		int iExpN = 2;
		int iReqN = 0;
		string encryptedString = "";
		for (int i = 0; i < 23; i++)
		{
			if (i == 5 || i == 11 || i == 17)
				encryptedString += "-";
			else if (i == 6 || i == 7)
			{
				if (expirationDate.Length > 0)
					encryptedString += refChars[intCheckSum[i - 6] + refEncryptionNumbers[iRefN]].ToString();
				else
					encryptedString += "$";
				iRefN++;
			}
			else if (i == 8 || i == 9)
			{
				encryptedString += refChars[intRequestNumberLen[i - 8] + refEncryptionNumbers[iRefN]].ToString();
				iRefN++;
			}
			else if (i >= 14 && i <= 16)
			{
				if (expirationDate.Length > 0)
					encryptedString += refChars[(intExpirationDate[iExpN] * 10 + intExpirationDate[iExpN + 1]) + refEncryptionNumbers[iRefN]].ToString();
				else
					encryptedString += "$";
				iExpN += 2;
				iRefN++;
			}
			else if (iReqN >= requestNumber.Length)
			{
				if (expirationDate.Length > 0)
					encryptedString += refChars[UnityEngine.Random.Range(1, 36)].ToString();
				else
					encryptedString += "$";
			}
			else
			{
				encryptedString += refChars[intRequestNumber[iReqN] + refEncryptionNumbers[iRefN]].ToString();
				iReqN++;
				iRefN++;
			}
		}
		return encryptedString;
	}

	// =================================================================================================================================================================
	/// <summary> Extraire la date d'expiration. </summary>

	public static System.DateTime GetExpirationDate(string accessKey)
	{
		return new System.DateTime(2000 + refChars.IndexOf(accessKey.Substring(14, 1)) - refEncryptionNumbers[12], refChars.IndexOf(accessKey.Substring(15, 1)) - refEncryptionNumbers[13],
										refChars.IndexOf(accessKey.Substring(16, 1)) - refEncryptionNumbers[14]);
	}

	// =================================================================================================================================================================
	/// <summary> Convertir en un nombre décimal. </summary>

	static int[] ConvertToInteger(string stringToConvert)
	{
		int[] intNumbers = new int[stringToConvert.Length];
		for (int i = 0; i < stringToConvert.Length; i++)
			intNumbers[i] = int.Parse(stringToConvert.Substring(i, 1), CultureInfo.InvariantCulture);
		return intNumbers;
	}

	// =================================================================================================================================================================
	/// <summary> Encrypter la clé d'activation. </summary>

	public static void WriteAccessKeyInFile(string fileName, string accessKey)
	{
		string encryptedAccessKey = "";
		int n = 0;
		for (int i = 0; i < 23; i++)
		{
			if (accessKey[i] != '-')
			{
				int value =	refChars.IndexOf(accessKey[i]) + refEncryptionNumbers[n];
				if (value >= refChars.Length)
					value -= refChars.Length;
				encryptedAccessKey += refChars[value].ToString();
				n++;
			}
		}
		System.IO.File.WriteAllText(fileName, encryptedAccessKey);
	}

	// =================================================================================================================================================================
	/// <summary> Encrypter la clé d'activation. </summary>

	public static string ReadAccessKeyFromFile(string fileName)
	{
		string encryptedAccessKey = System.IO.File.ReadAllText(fileName);
		if (encryptedAccessKey.Length != 20)
			return "";

		string accessKey = "";
		for (int i = 0; i < 20; i++)
		{
			int value = refChars.IndexOf(encryptedAccessKey[i]) - refEncryptionNumbers[i];
			if (value < 0)
				value += refChars.Length;
			accessKey += refChars[value].ToString();
			if (i == 4 || i == 9 || i == 14)
				accessKey += "-";
		}
		return accessKey;
	}
}
