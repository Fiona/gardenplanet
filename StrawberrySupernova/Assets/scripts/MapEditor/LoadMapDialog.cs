using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadMapDialog : MonoBehaviour
{

    public GameObject buttonTemplate;
    public GameObject scrollContent;

    private bool close;
    private Ref<string> nameStore;

    public void Awake()
    {
        buttonTemplate.SetActive(false);
    }

    public IEnumerator Show(Ref<string> nameStore)
    {

        this.nameStore = nameStore;

        gameObject.SetActive(true);

        // Set up buttons
        foreach(Transform button in scrollContent.transform)
            if(button.gameObject.activeSelf)
                Destroy(button.gameObject);

        var dirPath = Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_MAPS);
        if(Directory.Exists(dirPath))
        {
            var mapFiles = Directory.GetFiles(dirPath, "*.map");
            foreach(var f in mapFiles)
            {
                var newButton = Instantiate(buttonTemplate);
                newButton.SetActive(true);
                newButton.GetComponentInChildren<Text>().text = f;
                newButton.transform.SetParent(scrollContent.transform, false);
                newButton.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        MapButtonPressed(f);
                    }
                );
            }
        }

        // Wait for button to be pressed
        close = false;
        while(!close)
            yield return new WaitForFixedUpdate();
        gameObject.SetActive(false);
    }

    public void CancelPressed()
    {
        close = true;
    }

    public void MapButtonPressed(string filename)
    {
        close = true;
        this.nameStore.Value = Path.GetFileNameWithoutExtension(filename);
    }

}
