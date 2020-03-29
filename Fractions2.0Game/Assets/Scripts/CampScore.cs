using UnityEngine;
using UnityEngine.UI;

public class CampScore : MonoBehaviour
{
	public VillagerCamp camp;
	public Text woodScore;
	public Text foodScore;
	public Text stoneScore;
    public Text newsUpdates;

    // Update is called once per frame
    void Update()
    {
		woodScore.text = camp.GetWoodCount().ToString();
		stoneScore.text = camp.GetStoneCount().ToString();
		foodScore.text = camp.GetFoodCount().ToString();
        newsUpdates.text = camp.GetNewsUpdates();
    }
}
