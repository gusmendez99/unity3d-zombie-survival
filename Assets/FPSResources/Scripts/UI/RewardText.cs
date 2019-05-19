using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardText : MonoBehaviour {
	public Text rewardText;

	int accumulatedExp = 0;
	int accumulatedFund = 0;

	IEnumerator updateRewardTextCo = null;

	void Start() {
        // find the RewardText and get component to assign it in rewardText
		rewardText = GameObject.Find("UI/InGameUI/Info/RewardText").GetComponent<Text>();
	}
    // shows the experience and the fund
	public void Show(int exp, int fund) {
		if(updateRewardTextCo != null) StopCoroutine(updateRewardTextCo);

		accumulatedExp += exp;
		accumulatedFund += fund;

		rewardText.text = "EXP +" + accumulatedExp + "\nFund +" + accumulatedFund + " $";

		updateRewardTextCo = Hide();
		StartCoroutine(updateRewardTextCo);
	}

	IEnumerator Hide() {
		yield return new WaitForSeconds(5f);
		rewardText.text = "";
		accumulatedExp = 0;
		accumulatedFund = 0;

		yield break;
	}
}
