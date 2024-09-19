using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class UIGameMenu : MonoBehaviour
{
    public GameObject nicknameInput;

    public void Start() {
        // Generate a random nickname for the player
        string nickname = this.GenerateRandomNickname();
        nicknameInput.GetComponent<UnityEngine.UI.InputField>().text = nickname;
    }

    private string GenerateRandomNickname()
    {
        return "Player" + Random.Range(100, 1000);
    }
}
