using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    TextMeshProUGUI text;
    Color orgColor;
    Image image;

    public AudioSource audioSource;

    public void Start()
    {
        text = transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
        image = GetComponent<Image>();
        orgColor = image.color;
    }

    public void MouseEnter()
    {
        text.fontStyle = FontStyles.Underline;
        image.color += new Color(0.2f, 0.2f, 0.2f, 1f);
        PlaySound();
    }

    public void MouseExit()
    {
        text.fontStyle = FontStyles.Normal;
        image.color = orgColor;
        PlaySound(false);
    }

    public void PlaySound(bool play=true)
    {
        if (play)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        } 
        else
        {
            audioSource.Stop();
        }            
    }
}