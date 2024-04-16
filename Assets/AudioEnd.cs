using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEnd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (AudioManager.Instance is not null)
        {
            AudioManager.Instance.EndMusic();
            AudioManager.Instance.PlaySoundEnd();
        }
    }
}
