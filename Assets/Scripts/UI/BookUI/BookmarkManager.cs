using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class BookmarkManager : MonoBehaviour
{
    // Start is called before the first frame update
    [field: SerializeField] public List<Bookmark> bookmarks = new();
    [SerializeField] private Sprite EnabledBookmark;
    
    public void UpdateBookmarkVisibility(List<Biome> biomes) 
    {
        if (ResourceGame.Instance.GetLevelSceneFlow() <= LevelSceneFlow.RelationPhase) return;
        for (int i = 0; i < biomes.Count; i++)
        {
            bookmarks[i].SetBiome(biomes[i].Type);
            bookmarks[i].CanvasGroup.alpha = 1.0f;
            bookmarks[i].CanvasGroup.interactable = true;
            bookmarks[i].CanvasGroup.blocksRaycasts = true;
            bookmarks[i].Image.sprite = EnabledBookmark;
        }

        for (int i = biomes.Count; i < bookmarks.Count; i++) 
        {
            bookmarks[i].CanvasGroup.alpha = 0.0f;
            bookmarks[i].CanvasGroup.interactable = false;
            bookmarks[i].CanvasGroup.blocksRaycasts = false;
        }
    }
}
