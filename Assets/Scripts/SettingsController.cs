using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SettingsController : MonoBehaviour
{
    public GameController _gc;

    public TextMeshProUGUI controlsToggleText;
    public TextMeshProUGUI movementControlHintText;
    public TextMeshProUGUI rotationControlHintText;
    public TextMeshProUGUI difficultyToggleText;

    public Dictionary<string, Dictionary<string, LocalizedString>> settingsDynamicText = new Dictionary<string, Dictionary<string, LocalizedString>>
    {
        {
            "movement",
            new Dictionary<string, LocalizedString>
            {
                {"default", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "HintMovementDefault"}},
                {"alternative", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "HintMovementAlt"}}
            }
        },
        {
            "rotation",
            new Dictionary<string, LocalizedString>
            {
                {"default", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "HintRotationDefault"}},
                {"alternative", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "HintRotationAlt"}}
            }
        },
        {
            "difficulty",
            new Dictionary<string, LocalizedString>
            {
                {"easy", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "DifficultyEasy"}},
                {"normal", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "DifficultyNormal"}},
                {"hard", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "DifficultyHard"}}
            }
        },
        {
            "controls",
            new Dictionary<string, LocalizedString>
            {
                {"default", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "ControlsDefault"}},
                {"alternative", new LocalizedString { TableReference = "PongLocalization", TableEntryReference = "ControlsAlternative"}}
            }
        }
    };

    private void OnEnable()
    {
        difficultyToggleText.text = settingsDynamicText["difficulty"][PlayerPrefs.GetString("Difficulty", "normal")].GetLocalizedString();

        controlsToggleText.text = settingsDynamicText["controls"][_gc.controlsType].GetLocalizedString();
        movementControlHintText.text = settingsDynamicText["movement"][_gc.controlsType].GetLocalizedString();
        rotationControlHintText.text = settingsDynamicText["rotation"][_gc.controlsType].GetLocalizedString();
    }

    public void ToggleSettingsPanel(bool value)
    {
        _gc.ToggleControls(value);
        _gc.ToggleControlsInteraction(value);
        ToggleTestPlatform(value);
    }

    public void ToggleDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty", "normal");
        switch (difficulty)
        {
            case "easy":
                difficulty = "normal";
                break;
            case "hard":
                difficulty = "easy";
                break;
            default:
                difficulty = "hard";
                break;
        }
        difficultyToggleText.text = settingsDynamicText["difficulty"][difficulty].GetLocalizedString();
        PlayerPrefs.SetString("Difficulty", difficulty);
        _gc.testPlatform.GetComponent<Singleplayer.PlatformController>().SetSpeedRatio(difficulty);
    }

    public void ToggleControlsType()
    {
        string controlsType = PlayerPrefs.GetString("ControlsType");
        if (controlsType == "alternative")
        {
            controlsType = "default";
            _gc.movementSlider.gameObject.SetActive(false);
            _gc.rotationSlider.gameObject.SetActive(false);
            _gc.controlLevers.SetActive(false);
        }
        else
        {
            controlsType = "alternative";
            _gc.movementSlider.gameObject.SetActive(true);
            _gc.rotationSlider.gameObject.SetActive(true);
            _gc.controlLevers.SetActive(true);
        }
        movementControlHintText.text = settingsDynamicText["movement"][controlsType].GetLocalizedString();
        rotationControlHintText.text = settingsDynamicText["rotation"][controlsType].GetLocalizedString();
        _gc.controlsType = controlsType;
        PlayerPrefs.SetString("ControlsType", controlsType);
        controlsToggleText.text = settingsDynamicText["controls"][controlsType].GetLocalizedString();
    }

    public void ToggleLanguage()
    {
        int selectedLanguageIndex = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            if (LocalizationSettings.AvailableLocales.Locales[i] == LocalizationSettings.SelectedLocale)
            {
                selectedLanguageIndex = i + 1;
                if (selectedLanguageIndex == LocalizationSettings.AvailableLocales.Locales.Count)
                {
                    selectedLanguageIndex = 0;
                }
                break;
            }
        }
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selectedLanguageIndex];
        PlayerPrefs.SetString("Language", LocalizationSettings.SelectedLocale.name);

        // Need to update other settings localizations
        controlsToggleText.text = settingsDynamicText["controls"][_gc.controlsType].GetLocalizedString();
        difficultyToggleText.text = settingsDynamicText["difficulty"][PlayerPrefs.GetString("Difficulty", "normal")].GetLocalizedString();
        movementControlHintText.text = settingsDynamicText["movement"][_gc.controlsType].GetLocalizedString();
        rotationControlHintText.text = settingsDynamicText["rotation"][_gc.controlsType].GetLocalizedString();
    }

    public void ToggleTestPlatform(bool value)
    {
        if (value)
        {
            _gc.testPlatform = Instantiate(_gc.playerPrefab, _gc.playerPrefab.transform.position, _gc.playerPrefab.transform.rotation);
            _gc.testPlatform.tag = "Player1";
            _gc.testPlatform.AddComponent<Singleplayer.PlatformController>().SetUp(0);
            _gc.testPlatform.AddComponent<Singleplayer.InputController>();
        }
        else
        {
            if (_gc.testPlatform != null)
            {
                Destroy(_gc.testPlatform);
            }
        }
    }

    public void ToggleDebugMode(bool newValue)
    {
        _gc.debugMode = newValue;
    }
}
