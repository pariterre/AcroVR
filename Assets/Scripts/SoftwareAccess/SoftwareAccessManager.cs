using UnityEngine;
using UnityEngine.UI;

// =================================================================================================================================================================
/// <summary> Activation/Désactivation de l'accès au logiciel AcroVR. </summary>

public class SoftwareAccessManager : MonoBehaviour
{
	public GameObject trampolineGameObject;
	public Text textExpirationDate;
	public GameObject panelSoftwareAccess;
	public Text textButtonChangeLanguage;
	public Text textButtonQuit;
	public Text textNoAccess;
	public Text textInstructions;
	public Text textRequestNumber;
	public Button buttonAllowAccess;
	public Image buttonAllowAccessImage;

	bool DisableAccessKey = true;	        // Façon rapide de désactiver rapidement tout la structure utilisant la clé d'accès

	string accessKeyFileName;				// Nom du fichier contenant la clé d'activation
	string longAccessKey;                   // Clé d'activation complète
	string shortAccessKey;                  // Clé d'activation raccourci (sans vérification de la somme, sans la date d'expiration et sams les extras)
	bool updateAccessKeyAuthorized;         // Autorisé la mise à jour de la clé d'activation
	bool expirationDateOverDue;             // Indique que la date d'expiration a été dépassé, au moins une fois
	System.DateTime expirationDateFile;		// Date d'expiration lue à partir du fichier

	InputField inputFieldAccessKey;

	/// <summary> Description de la structure contenant la liste des messages utilisés. </summary>
	public struct StrucMessageLists
	{
		public string textExpirationDate;
		public string textNoAccess1;
		public string textNoAccess2;
		public string textInstructions;
		public string buttonAllowAccess;
		public string buttonQuit;
	}

	/// <summary> Description de la structure contenant la liste des messages utilisés en français et en anglais. </summary>
	public struct StrucLanguages
	{
		public StrucMessageLists Used;
		public StrucMessageLists french;
		public StrucMessageLists english;
	}

	/// <summary> Structure contenant la liste des messages utilisés en français et en anglais. </summary>
	public StrucLanguages languages;

	// =================================================================================================================================================================
	/// <summary> Initialisation du script. </summary>

	void Awake()
	{
		if (DisableAccessKey)
		{
			textExpirationDate.text = "";
			return;
		}

		// Initialisation des messages utilisées dans chacune des langues utilisées

		InitLanguages();

		// Initialisation de l'affichage de la page utilisé pour avoir accès au logiciel

		buttonAllowAccess.interactable = false;
		buttonAllowAccessImage.color = new Color(150, 150, 150);

		// Déterminer la pseudo clé d'activation qu'on va pouvoir comparer à celle contenu dans le fichier

		string infos = string.Format("{0}{1}{2}{3}{4}{5}", SystemInfo.deviceUniqueIdentifier.ToString(), SystemInfo.processorType.ToString(), SystemInfo.processorFrequency.ToString(),
																		SystemInfo.processorCount.ToString(), SystemInfo.operatingSystem.ToString(), SystemInfo.deviceModel.ToString());
		string requestNumber = GetAccessKey.EncryptedComputerInfos(infos);
		textRequestNumber.text = requestNumber;
		shortAccessKey = GetAccessKey.EncryptedAccessKey("", requestNumber);

		// Vérifier si le fichier contenant la clé d'activation existe, sinon alors afficher la page utilisé pour avoir accès au logiciel

#if UNITY_STANDALONE_OSX
		string dirCheckFileName = string.Format("{0}/Documents/AcroVR/Lib", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
		accessKeyFileName = string.Format("{0}/AcroVR.dll", dirCheckFileName);
#else
		accessKeyFileName = Application.persistentDataPath + @"/AcroVR AccessKey.txt";
#endif
		if (!System.IO.File.Exists(accessKeyFileName))
		{
			longAccessKey = "";
			expirationDateOverDue = false;
			expirationDateFile = new System.DateTime(2999,1,1);
			trampolineGameObject.SetActive(false);                                      // Désactivé tous les gameobjects visibles, pour avoir un écran vide
			panelSoftwareAccess.SetActive(true);                                        // Activé le panneau utilisé pour avoir accès au logiciel
			return;
		}

		// Lecture de la clé d'activation contenu dans le fichier et en extraire les informations nécessaires

		longAccessKey =	GetAccessKey.ReadAccessKeyFromFile(accessKeyFileName);
		expirationDateOverDue = longAccessKey.Substring(longAccessKey.Length - 1, 1) == "A";
		updateAccessKeyAuthorized = !expirationDateOverDue;
		expirationDateFile = GetAccessKey.GetExpirationDate(longAccessKey);
		if (!CheckAccessKey())
		{
			trampolineGameObject.SetActive(false);
			panelSoftwareAccess.SetActive(true);
			return;
		}
		textExpirationDate.text = languages.Used.textExpirationDate + GetAccessKey.GetExpirationDate(longAccessKey).ToShortDateString();
	}

	// =================================================================================================================================================================
	/// <summary> Bouton Changement de langue utilisée a été appuyer. </summary>

	public void ButtonChangeLanguage()
	{
		if (textButtonChangeLanguage.text == "Fr")
		{
			languages.Used = languages.french;
			textButtonChangeLanguage.text = "En";
		}
		else
		{
			languages.Used = languages.english;
			textButtonChangeLanguage.text = "Fr";
		}

		if (textNoAccess.text == languages.french.textNoAccess1 || textNoAccess.text == languages.english.textNoAccess1)
			textNoAccess.text = languages.Used.textNoAccess1;
		else if (textNoAccess.text == languages.french.textNoAccess2 || textNoAccess.text == languages.english.textNoAccess2)
			textNoAccess.text = languages.Used.textNoAccess2;

		textInstructions.text = languages.Used.textInstructions;
		buttonAllowAccess.GetComponentInChildren<Text>().text = languages.Used.buttonAllowAccess;
		textButtonQuit.text = languages.Used.buttonQuit;
	}

	// =================================================================================================================================================================
	/// <summary> Vérifier si la clé d'activation est valide. </summary>

	public void CheckAccessKey(string accessKey)
	{
		if (accessKey.Length == shortAccessKey.Length)
			updateAccessKeyAuthorized = GetAccessKey.GetExpirationDate(accessKey) > expirationDateFile || !expirationDateOverDue;
		else
			updateAccessKeyAuthorized = true;
		longAccessKey = accessKey;
		CheckAccessKey();
	}

	bool CheckAccessKey()
	{
		if (longAccessKey.Length != shortAccessKey.Length)
		{
			textNoAccess.text = languages.Used.textNoAccess1;
			buttonAllowAccess.interactable = false;
			buttonAllowAccessImage.color = new Color(150, 150, 150);
			longAccessKey = "";
			return false;
		}
		if (System.DateTime.Today >= GetAccessKey.GetExpirationDate(longAccessKey) || !updateAccessKeyAuthorized)
		{
			textNoAccess.text = languages.Used.textNoAccess2;
			buttonAllowAccess.interactable = false;
			buttonAllowAccessImage.color = new Color(150, 150, 150);
			if (System.IO.File.Exists(accessKeyFileName))
				GetAccessKey.WriteAccessKeyInFile(accessKeyFileName, longAccessKey.Substring(0, longAccessKey.Length - 1) + "A");
			longAccessKey = "";
			return false;
		}

		int i = 0;
		while (i < longAccessKey.Length && (shortAccessKey[i] == '$' || longAccessKey[i] == shortAccessKey[i]))
			i++;
		if (i >= longAccessKey.Length)
		{
			textNoAccess.text = "";
			buttonAllowAccess.interactable = true;
			buttonAllowAccessImage.color = Color.white;
		}
		else
		{
			textNoAccess.text = languages.Used.textNoAccess1;
			buttonAllowAccess.interactable = false;
			buttonAllowAccessImage.color = new Color(150, 150, 150);
			longAccessKey = "";
			return false;
		}
		return true;
	}

	// =================================================================================================================================================================
	/// <summary> Bouton Activer l'accès a été appuyer (Autorise l'accès au logiciel). </summary>

	public void AccessSoftware()
	{
		GetAccessKey.WriteAccessKeyInFile(accessKeyFileName, longAccessKey);
		textExpirationDate.text = languages.Used.textExpirationDate + GetAccessKey.GetExpirationDate(longAccessKey).ToShortDateString();
		trampolineGameObject.SetActive(true);
		panelSoftwareAccess.SetActive(false);
	}

	// =================================================================================================================================================================
	/// <summary> Bouton OK a été appuyer (Quitte le logiciel). </summary>

	public void ButtonOK()
	{
		Application.Quit();
	}

	// =================================================================================================================================================================
	/// <summary> Initialisation des messages utilisées dans chacune des langues utilisées. </summary>

	void InitLanguages()
	{
		languages.french.textExpirationDate = "Date d'expiration: ";
		languages.english.textExpirationDate = "Expiration date: ";
		languages.french.textNoAccess1 = "Accès désactivé: Clé d'accès invalide";
		languages.english.textNoAccess1 = "Access denied: Access key invalid";
		languages.french.textNoAccess2 = "Accès désactivé: Date d'expiration dépassée";
		languages.english.textNoAccess2 = "Access denied: Expiration date over due";

		languages.french.textInstructions = string.Format("{0}{1}{2}{3}{4}{5}{6}",
			"Pour activer le logiciel, transmettre le numéro de requête suivant", System.Environment.NewLine,
			"à votre personne contact du groupe S2M.", System.Environment.NewLine,
			"Une clé d'activation vous sera envoyé en retour,", System.Environment.NewLine,
			"que vous devrez entrer dans la boîte ci-dessous.");
		languages.english.textInstructions = string.Format("{0}{1}{2}{3}{4}{5}{6}",
			"To enable the software, communicate the following request number", System.Environment.NewLine,
			"at your contact person in the S2M group.", System.Environment.NewLine,
			"An access key will be sent to you in return,", System.Environment.NewLine,
			"that you will need to enter in the box below.");

		languages.french.buttonAllowAccess = "Activer l'accès";
		languages.english.buttonAllowAccess = "Enable access";

		languages.french.buttonQuit = "Quitter";
		languages.english.buttonQuit = "Quit";

		// Initialisation des textes

		textNoAccess.text = "";
		textButtonChangeLanguage.text = "Fr";
		ButtonChangeLanguage();
	}
}
