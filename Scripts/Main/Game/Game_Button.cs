using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Button : MonoBehaviour
{
    [SerializeField]
    GameObject DiceEyesText = default;

    private Button button;
    private GameSceneManager gameSceneManager;
    private Text eyesText;

    // Start is called before the first frame update
    void Start()
    {
        gameSceneManager = FindObjectOfType<GameSceneManager>();
        eyesText = DiceEyesText.GetComponent<Text>();
        eyesText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*#======================================================================#*/
    /*#    function : RolltheDice  function                                  #*/
    /*#    summary  : さいころをぶん回す                                     #*/
    /*#    argument : int            index            -  マスインデックス    #*/
    /*#               squareKindDef  kind             -  マス種別            #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void RollTheDice()
    {
        int eyes;
        eyes = Random.Range(1,11);

        eyesText.text = eyes.ToString();

        gameSceneManager.DeliveryDiceTheEyes(eyes);

        button.interactable = false;
    }

    /*#======================================================================#*/
    /*#    function : SwitchButtonDisplay                                    #*/
    /*#    summary  : ボタン表示を切り替える                                 #*/
    /*#    argument : bool  displayFlg                -  表示/非表示フラグ   #*/
    /*#    return   : nothing                                                #*/
    /*#======================================================================#*/
    public void SwitchButtonDisplay(bool displayFlg)
    {
        button = GetComponent<Button>();
        button.interactable = displayFlg;
    }
}
