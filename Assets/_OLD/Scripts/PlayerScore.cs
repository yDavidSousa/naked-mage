using UnityEngine;

public sealed class PlayerScore : MonoBehaviour {
    //InputField input;
    void Start () {
        //input = gameObject.GetComponent<InputField>();
        //input.onEndEdit.AddListener(CallAddScore); 
    }

    private void CallAddScore (string arg0) {
        if (arg0.Length > 0)
            GameManager.instance.SaveScore(arg0);
        else 
            GameManager.instance.SaveScore("anon");
    }
}
