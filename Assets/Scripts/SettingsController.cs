using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class SettingsController : MonoBehaviour
{
    public GameController _gc;

    public TextMeshProUGUI controlsToggleText;
    public TextMeshProUGUI movementControlHintText;
    public TextMeshProUGUI rotationControlHintText;
    public TextMeshProUGUI difficultyToggleText;

    public List<GameObject> localizableObjects;
    public TMP_Dropdown gameModeDropdown;

    public Dictionary<string, Dictionary<string, LocalizedString>> settingsDynamicText = new Dictionary<string, Dictionary<string, LocalizedString>>
    {
        {
            "movement",
            new Dictionary<string, LocalizedString>
            {
                {"default", new LocalizedString("PongLocalization", "HintMovementDefault")},
                {"alternative", new LocalizedString("PongLocalization", "HintMovementAlt")}
            }
        },
        {
            "rotation",
            new Dictionary<string, LocalizedString>
            {
                {"default", new LocalizedString("PongLocalization", "HintRotationDefault")},
                {"alternative", new LocalizedString("PongLocalization", "HintRotationAlt")}
            }
        },
        {
            "difficulty",
            new Dictionary<string, LocalizedString>
            {
                {"easy", new LocalizedString("PongLocalization", "DifficultyEasy")},
                {"normal", new LocalizedString("PongLocalization", "DifficultyNormal")},
                {"hard", new LocalizedString("PongLocalization", "DifficultyHard")}
            }
        },
        {
            "controls",
            new Dictionary<string, LocalizedString>
            {
                {"default", new LocalizedString("PongLocalization", "ControlsDefault")},
                {"alternative", new LocalizedString("PongLocalization", "ControlsAlternative")}
            }
        }
    };
    public List<LocalizedString> gameModeLocalizedNames = new List<LocalizedString>
    {
        new LocalizedString("PongLocalization", "GameModeClassic"),
        new LocalizedString("PongLocalization", "GameModeAccuracy")
    };

    private void OnEnable()
    {
        difficultyToggleText.text = settingsDynamicText["difficulty"][PlayerPrefs.GetString("Difficulty", "normal")].GetLocalizedString();

        controlsToggleText.text = settingsDynamicText["controls"][_gc.controlsType].GetLocalizedString();
        movementControlHintText.text = settingsDynamicText["movement"][_gc.controlsType].GetLocalizedString();
        rotationControlHintText.text = settingsDynamicText["rotation"][_gc.controlsType].GetLocalizedString();
    }

    public void ToggleControlsObejcts(bool value)
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

    public IEnumerator SetLanguage(string language)
    {
        yield return LocalizationSettings.InitializationOperation;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.name == language)
            {
                LocalizationSettings.SelectedLocale = locale;
                PlayerPrefs.SetString("Language", locale.name);
                break;
            }
        }
        UpdateLocalizedFields();
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

        StartCoroutine(SetLanguage(LocalizationSettings.AvailableLocales.Locales[selectedLanguageIndex].name));
    }

    public void UpdateLocalizedFields()
    {
        controlsToggleText.text = settingsDynamicText["controls"][_gc.controlsType].GetLocalizedString();
        difficultyToggleText.text = settingsDynamicText["difficulty"][PlayerPrefs.GetString("Difficulty", "normal")].GetLocalizedString();
        movementControlHintText.text = settingsDynamicText["movement"][_gc.controlsType].GetLocalizedString();
        rotationControlHintText.text = settingsDynamicText["rotation"][_gc.controlsType].GetLocalizedString();

        foreach (var localizedObject in localizableObjects)
        {
            localizedObject.GetComponent<GameObjectLocalizer>().ApplyLocaleVariant(LocalizationSettings.SelectedLocale);
        }
        for (int i = 0; i < gameModeLocalizedNames.Count; i++)
        {
            gameModeDropdown.options[i].text = gameModeLocalizedNames[i].GetLocalizedString();
        }
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
