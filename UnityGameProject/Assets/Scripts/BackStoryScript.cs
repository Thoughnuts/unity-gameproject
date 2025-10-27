using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackstoryManager : MonoBehaviour
{
    public Text storyText;
    public Image storyImage;
    public string fullStory;
    public float typingSpeed = 0.05f;
    public Button continueButton;

    private bool isTyping = false;
    private bool skipRequested = false;

    void Start()
    {
        if (storyText == null)
            storyText = GameObject.Find("StoryText").GetComponent<Text>();

        if (storyImage == null)
            storyImage = GameObject.Find("StoryImage").GetComponent<Image>();

        if (continueButton == null)
            continueButton = GameObject.Find("ContinueButton").GetComponent<Button>();

        // Configure text for auto-sizing
        storyText.resizeTextForBestFit = true;
        storyText.resizeTextMinSize = 10;
        storyText.resizeTextMaxSize = 24;
        storyText.verticalOverflow = VerticalWrapMode.Overflow;

        continueButton.gameObject.SetActive(false);
        continueButton.onClick.AddListener(LoadNextScene);

        StartCoroutine(TypeStory());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
                skipRequested = true;
        }
    }

    IEnumerator TypeStory()
    {
        isTyping = true;
        storyText.text = "";

        foreach (char letter in fullStory.ToCharArray())
        {
            if (skipRequested)
            {
                storyText.text = fullStory;
                break;
            }

            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        skipRequested = false;
        continueButton.gameObject.SetActive(true);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(3);
    }
}