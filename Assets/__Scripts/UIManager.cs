using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour{

    public Animator startButton;
    public Animator settingsButton;
    public Animator dialog;

    public void StartGame(){
        SceneManager.LoadScene("__Bartok_Scene_0");
    }

    public void OpenSettings(){
        startButton.SetBool("isHidden", true);
        settingsButton.SetBool("isHidden", true);
        dialog.SetBool("isHidden", false);
    }

    public void CloseSettings(){
        startButton.SetBool("isHidden", false);
        settingsButton.SetBool("isHidden", false);
        dialog.SetBool("isHidden", true);
    }


}
