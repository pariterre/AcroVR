using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguage : MonoBehaviour
{
    public struct StrucLanguages
    {
        public StrucMessageLists french;
        public StrucMessageLists english;
    }

    public struct StrucMessageLists
    {
        public string Training;
        public string TrainingSub;
        public string Setting;
        public string SettingSub;
        public string Quit;
        public string QuitSub;

        public string SettingTitle;
        public string Close;
        public string Language;
        public string Tooltip;
        public string MaleFemale;

        public string ChooseLanguage;
        public string English;
        public string French;

        public string TooltipTitle;
        public string ChooseTooltip;
        public string Yes;
        public string No;

        public string MaleFemaleTitle;
        public string ChooseTooltipMaleFemale;
        public string Male;
        public string Female;

        public string ExitTitle;
    }

    public StrucLanguages languages;

    // Main Menu
    public Text Training;
    public Text TrainingSub;
    public Text hTraining;
    public Text hTrainingSub;
    public Text Setting;
    public Text SettingSub;
    public Text hSetting;
    public Text hSettingSub;
    public Text Quit;
    public Text QuitSub;
    public Text hQuit;
    public Text hQuitSub;

    // Setting
    public Text SettingTitle;
    public Text SettingClose;
    public Text hSettingClose;
    public Text Language;
    public Text hLanguage;
    public Text Tooltip;
    public Text hTooltip;
    public Text MaleFemale;
    public Text hMaleFemale;

    //Language
    public Text ChooseLanguage;
    public Text LanguageClose;
    public Text hLanguageClose;
    public Text English;
    public Text hEnglish;
    public Text French;
    public Text hFrench;

    //Tooltip
    public Text TooltipTitle;
    public Text ChooseTooltip;
    public Text TooltipYes;
    public Text hTooltipYes;
    public Text TooltipNo;
    public Text hTooltipNo;

    //Male & Female
    public Text MaleFemaleTitle;
    public Text ChooseMaleFemale;
    public Text Male;
    public Text hMale;
    public Text Female;
    public Text hFemale;

    //Exit menu
    public Text ExitTitle;
    public Text ExitYes;
    public Text hExitYes;
    public Text ExitNo;
    public Text hExitNo;

    private void Start()
    {
        languages.english.Training = "Training";
        languages.french.Training = "Training";
        languages.english.TrainingSub = "Practice and Free Play";
        languages.french.TrainingSub = "Pratique et jeu libre";
        languages.english.Setting = "Settings";
        languages.french.Setting = "Réglages";
        languages.english.SettingSub = "Set up Options";
        languages.french.SettingSub = "Configuration des options";
        languages.english.Quit = "Quit";
        languages.french.Quit = "Quitter";
        languages.english.QuitSub = "Exit Game";
        languages.french.QuitSub = "Quitter le logiciel";

        languages.english.SettingTitle = "SETTING";
        languages.french.SettingTitle = "RÉGLAGE";
        languages.english.Close = "CLOSE";
        languages.french.Close = "FERMER";
        languages.english.Language = "Language";
        languages.french.Language = "Langue";
        languages.english.Tooltip = "Tool tips";
        languages.french.Tooltip = "Info-bulles";
        languages.english.MaleFemale = "Male&Female";
        languages.french.MaleFemale = "Homme&Femme";

        languages.english.ChooseLanguage = "Select your language";
        languages.french.ChooseLanguage = "Sélectionnez votre langue";
        languages.english.English = "English";
        languages.french.English = "Anglais";
        languages.english.French = "French";
        languages.french.French = "Français";

        languages.english.TooltipTitle = "Tooltip";
        languages.french.TooltipTitle = "Info-bulle";
        languages.english.ChooseTooltip = "Do you want to see Tooltips?";
        languages.french.ChooseTooltip = "Voulez-vous voir les info-bulles?";
        languages.english.Yes = "Yes";
        languages.french.Yes = "Oui";
        languages.english.No = "No";
        languages.french.No = "Non";

        languages.english.MaleFemaleTitle = "Male & Female";
        languages.french.MaleFemaleTitle = "Homme & Femme";
        languages.english.ChooseTooltipMaleFemale = "Choose 3D avatar";
        languages.french.ChooseTooltipMaleFemale = "Choisissez l'avatar 3D";
        languages.english.Male = "Male";
        languages.french.Male = "Homme";
        languages.english.Female = "Female";
        languages.french.Female = "Femme";

        languages.english.ExitTitle = "Do you want to exit?";
        languages.french.ExitTitle = "Voulez-vous quitter?";

        ChangedLanguage();
    }
    public void ChangedLanguage()
    {
        MainParameters.StrucMessageLists languagesUsed = MainParameters.Instance.languages.Used;
        if (languagesUsed.toolTipButtonQuit == "Quit")
        {
            Training.text = languages.english.Training;
            hTraining.text = languages.english.Training;
            TrainingSub.text = languages.english.TrainingSub;
            hTrainingSub.text = languages.english.TrainingSub;
            Setting.text = languages.english.Setting;
            hSetting.text = languages.english.Setting;
            SettingSub.text = languages.english.SettingSub;
            hSettingSub.text = languages.english.SettingSub;
            Quit.text = languages.english.Quit;
            hQuit.text = languages.english.Quit;
            QuitSub.text = languages.english.QuitSub;
            hQuitSub.text = languages.english.QuitSub;

            SettingTitle.text = languages.english.SettingTitle;
            SettingClose.text = languages.english.Close;
            hSettingClose.text = languages.english.Close;
            Language.text = languages.english.Language;
            hLanguage.text = languages.english.Language;
            Tooltip.text = languages.english.Tooltip;
            hTooltip.text = languages.english.Tooltip;
            MaleFemale.text = languages.english.MaleFemale;
            hMaleFemale.text = languages.english.MaleFemale;

            ChooseLanguage.text = languages.english.ChooseLanguage;
            LanguageClose.text = languages.english.Close;
            hLanguageClose.text = languages.english.Close;
            English.text = languages.english.English;
            hEnglish.text = languages.english.English;
            French.text = languages.french.French;
            hFrench.text = languages.french.French;

            TooltipTitle.text = languages.english.TooltipTitle;
            ChooseTooltip.text = languages.english.ChooseTooltip;
            TooltipYes.text = languages.english.Yes;
            hTooltipYes.text = languages.english.Yes;
            TooltipNo.text = languages.english.No;
            hTooltipNo.text = languages.english.No;

            MaleFemaleTitle.text = languages.english.MaleFemaleTitle;
            ChooseMaleFemale.text = languages.english.ChooseTooltipMaleFemale;
            Male.text = languages.english.Male;
            hMale.text = languages.english.Male;
            Female.text = languages.english.Female;
            hFemale.text = languages.english.Female;

            ExitTitle.text = languages.english.ExitTitle;
            ExitYes.text = languages.english.Yes;
            hExitYes.text = languages.english.Yes;
            ExitNo.text = languages.english.No;
            hExitNo.text = languages.english.No;
        }
        else
        {
            Training.text = languages.french.Training;
            hTraining.text = languages.french.Training;
            TrainingSub.text = languages.french.TrainingSub;
            hTrainingSub.text = languages.french.TrainingSub;
            Setting.text = languages.french.Setting;
            hSetting.text = languages.french.Setting;
            SettingSub.text = languages.french.SettingSub;
            hSettingSub.text = languages.french.SettingSub;
            Quit.text = languages.french.Quit;
            hQuit.text = languages.french.Quit;
            QuitSub.text = languages.french.QuitSub;
            hQuitSub.text = languages.french.QuitSub;

            SettingTitle.text = languages.french.SettingTitle;
            SettingClose.text = languages.french.Close;
            hSettingClose.text = languages.french.Close;
            Language.text = languages.french.Language;
            hLanguage.text = languages.french.Language;
            Tooltip.text = languages.french.Tooltip;
            hTooltip.text = languages.french.Tooltip;
            MaleFemale.text = languages.french.MaleFemale;
            hMaleFemale.text = languages.french.MaleFemale;

            ChooseLanguage.text = languages.french.ChooseLanguage;
            LanguageClose.text = languages.french.Close;
            hLanguageClose.text = languages.french.Close;
            English.text = languages.english.English;
            hEnglish.text = languages.english.English;
            French.text = languages.french.French;
            hFrench.text = languages.french.French;

            TooltipTitle.text = languages.french.TooltipTitle;
            ChooseTooltip.text = languages.french.ChooseTooltip;
            TooltipYes.text = languages.french.Yes;
            hTooltipYes.text = languages.french.Yes;
            TooltipNo.text = languages.french.No;
            hTooltipNo.text = languages.french.No;

            MaleFemaleTitle.text = languages.french.MaleFemaleTitle;
            ChooseMaleFemale.text = languages.french.ChooseTooltipMaleFemale;
            Male.text = languages.french.Male;
            hMale.text = languages.french.Male;
            Female.text = languages.french.Female;
            hFemale.text = languages.french.Female;

            ExitTitle.text = languages.french.ExitTitle;
            ExitYes.text = languages.french.Yes;
            hExitYes.text = languages.french.Yes;
            ExitNo.text = languages.french.No;
            hExitNo.text = languages.french.No;
        }
    }
}
