using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ArrangeObstacleBtnsPanel : MonoBehaviour, IInitializable
{
    [Inject]
    public void Construct(
        GameSettingsInstaller.GameSettings gameSettings, 
        ObstacleBtn.Factory obstacleBtnFactory,
        ToolGroupPanel toolGroupPanel
        )
    {
        this.gameSettings = gameSettings;
        this.obstacleBtnFactory = obstacleBtnFactory;
        this.toolGroupPanel = toolGroupPanel;
    }

    GameSettingsInstaller.GameSettings gameSettings;
    ObstacleBtn.Factory obstacleBtnFactory;
    
    [SerializeField] private Transform solidGroup;
    [SerializeField] private Transform harmfulGroup;
    [SerializeField] private Transform propsGroup;
    private ToolGroupPanel toolGroupPanel;

    public ObstacleModel CurrentObstacleData { get; private set; }

    private Button[] buttons;
    private Image[] btnBGImages;

    public void Initialize()
    {
        List<Button> buttons = new List<Button>();
        buttons.AddRange(solidGroup.GetComponentsInChildren<Button>());
        buttons.AddRange(harmfulGroup.GetComponentsInChildren<Button>());
        buttons.AddRange(propsGroup.GetComponentsInChildren<Button>());

        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i].gameObject);
        }

        CreateObstacleBtns();
    }

    private void CreateObstacleBtns()
    {
        var obstacles = gameSettings.obstacleDatas;
        buttons = new Button[obstacles.Length];
        btnBGImages = new Image[obstacles.Length];
        for (int i = 0; i < obstacles.Length; i++)
        {
            var obstacle = obstacles[i];
            var obstacleType = obstacle.obstacleType;

            var obstacleBtnInstance = obstacleBtnFactory.Create().gameObject;

            btnBGImages[i] = obstacleBtnInstance.GetComponent<Image>();
            var image = obstacleBtnInstance.transform.GetChild(0).GetComponent<Image>();


            image.sprite = obstacle.sprite;

            buttons[i] = obstacleBtnInstance.GetComponentInChildren<Button>();
            int j = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(obstacles, j));


            switch (obstacleType)
            {
                case ObstacleType.Obstacle:
                    obstacleBtnInstance.transform.SetParent(solidGroup);
                    break;
                case ObstacleType.Harmful:
                    obstacleBtnInstance.transform.SetParent(harmfulGroup);
                    break;
                case ObstacleType.Prop:
                    obstacleBtnInstance.transform.SetParent(propsGroup);
                    break;
                default:
                    break;
            }

        }

        //first click when startup
        OnButtonClick(obstacles, 0);
    }

    private void OnButtonClick(ObstacleModel[] obstacles, int index)
    {
        toolGroupPanel.ChangeToolType(ToolType.BaseSelection);

        CurrentObstacleData = obstacles[index];

        for (int k = 0; k < buttons.Length; k++)
        {
            btnBGImages[k].color = Color.gray;
        }
        btnBGImages[index].color = new Color(0.28f, 0.35f, 1);
    }

   
}
