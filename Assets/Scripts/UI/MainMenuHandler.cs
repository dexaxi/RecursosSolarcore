using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AnKuchen.Map;
using Cysharp.Threading.Tasks;
using System.Reflection;
using DUJAL.Systems.Audio;

public class MainMenuHandler : MonoBehaviour
{
    [field: SerializeField] private UICache mainScreenCache = default;

    private void Start()
    {
        MainScreenSetup();
        AudioManager.Instance.Play("Musica_Fondo");
    }

    private void MainScreenSetup() 
    {
        var mainUI = new MainMenuUIElements(mainScreenCache);
        mainUI.Root.SetActive(true);
    }

    public class MainMenuUIElements : IMappedObject
    {
        public IMapper Mapper { get; private set; }
        public GameObject Root { get; private set; }
        public Button Play_But_Button { get; private set; }
        public Button Quit_But_Button { get; private set; }
        public Button Settings_But_Button { get; private set; }
        public Button Controls_But_Button { get; private set; }

        public MainMenuUIElements() { }
        public MainMenuUIElements(IMapper mapper) { Initialize(mapper); }

        public void Initialize(IMapper mapper)
        {
            Mapper = mapper;
            Root = mapper.Get();
            Play_But_Button = mapper.Get<Button>("BTN_Play_Button");
            Quit_But_Button = mapper.Get<Button>("BTN_Quit_Button");
            Play_But_Button.onClick.AddListener(Play);
            Quit_But_Button.onClick.AddListener(QuitGame);
        }
        private void Play()
        {
            SceneLoader.Instance.LoadScene(SceneIndex.CHARACTER_SELECTOR, 300, false);
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
