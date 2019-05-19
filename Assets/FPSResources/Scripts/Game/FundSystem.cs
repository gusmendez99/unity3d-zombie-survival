using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundSystem : MonoBehaviour {
    // fund from player
	private int fund = 0;
    // Text to show fund
	public Text fundText;

	void Start() {
        // find FundText objcect and assign to fundText
		fundText = GameObject.Find("UI/InGameUI/CharacterStatus/FundText").GetComponent<Text>();
        // update the UI
		UpdateUI();
	}
    // shows fund
	void UpdateUI() {
		fundText.text = "Fund: " + fund.ToString() + " $";
	}
    // getsFund
	public int GetFund() {
		return fund;
	}
    // adds a certain amount of fund
	public void AddFund(int amount) {
		fund += amount;
		UpdateUI();
	}
    // rest an amount of fund
	public void TakeFund(int amount) {
		fund -= amount;
		UpdateUI();
	}
}
