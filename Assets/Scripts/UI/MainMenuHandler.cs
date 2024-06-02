using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AnKuchen.Map;
using Cysharp.Threading.Tasks;
using System.Reflection;

public class MainMenuHandler : MonoBehaviour
{
    [field: SerializeField] private UICache mainScreenCache = default;

    private void Start()
    {
        MainScreenSetup();
    }

    private void MainScreenSetup() 
    {
        var mainUI = new IPad1UiElements(mainScreenCache);
        mainUI.Root.SetActive(true);
    }

    public class IPad1UiElements : IMappedObject
    {
        public IMapper Mapper { get; private set; }
        public GameObject Root { get; private set; }
        public Button Play_But_Button { get; private set; }
        public Button Quit_But_Button { get; private set; }
        public Button Settings_But_Button { get; private set; }
        public Button Controls_But_Button { get; private set; }

        public IPad1UiElements() { }
        public IPad1UiElements(IMapper mapper) { Initialize(mapper); }

        public void Initialize(IMapper mapper)
        {
            Mapper = mapper;
            Root = mapper.Get();
            Play_But_Button = mapper.Get<Button>("Play_But_Button");
            Quit_But_Button = mapper.Get<Button>("Quit_But_Button");
            Settings_But_Button = mapper.Get<Button>("Settings_But_Button");
            Controls_But_Button = mapper.Get<Button>("Controls_But_Button");
            Play_But_Button.onClick.AddListener(Play);
            Quit_But_Button.onClick.AddListener(QuitGame);
            Settings_But_Button.onClick.AddListener(LoadSettings);
            Controls_But_Button.onClick.AddListener(LoadControls);
        }
        private void Play()
        {
            SceneLoader.Instance.LoadScene(SceneIndex.LEVEL_SELECTOR);
        }
        private void QuitGame()
        {
            Application.Quit();
        }

        private void LoadSettings()
        {
            Debug.Log("Settings!");
        }

        private void LoadControls()
        {
            Debug.Log("Controls!");
        }
    }
}
