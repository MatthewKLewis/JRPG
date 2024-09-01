using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class sJournalMenu : MonoBehaviour
{
    private GameManager gM;

    private int currentPage;
    [SerializeField] private TextMeshProUGUI pageText;
    [SerializeField] private TextMeshProUGUI pageNumberText;

    private readonly int CHUNK_SIZE = 600;
    private List<string> journalPageContent;

    void Start()
    {
        gM = GameManager.instance;

        //Split and flip to past page on Start.
        journalPageContent = SplitBy(gM.activeSave.journalContents, CHUNK_SIZE).ToList();
        currentPage = journalPageContent.Count - 1;

        LoadPageText();
    }

    //VIEW
    private void LoadPageText()
    {
        pageNumberText.text = (currentPage + 1).ToString();
        pageText.text = "";
        if (journalPageContent[currentPage] != null)
        {
            pageText.text = journalPageContent[currentPage];
        }
    }

    
    //BUTTONS
    public void FlipPageButton (bool forwards)
    {
        //Iterate and clamp
        if (forwards) { currentPage++; } 
        else { currentPage--; }
        currentPage = Mathf.Clamp(currentPage, 0, journalPageContent.Count-1);

        LoadPageText();
    }


    //UTILITY
    public IEnumerable<string> SplitBy(string str, int chunkLength)
    {
        for (int i = 0; i < str.Length; i += chunkLength)
        {
            if (chunkLength + i > str.Length)
                chunkLength = str.Length - i;

            yield return str.Substring(i, chunkLength);
        }
    }
}
