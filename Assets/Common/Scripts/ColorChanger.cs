using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour {
	public ColorProvider provider;

	[SerializeField] private new Renderer renderer;
	[SerializeField] private Button button;

	[SerializeField] private ColorBlock buttonColorBlock;

	[SerializeField] private bool checkOnUpdate = true;

	public void Awake() {
		renderer = GetComponent<Renderer>();
		provider = GetComponent<ColorProvider>();
		button = GetComponent<Button>();
		if (button != null) {
			buttonColorBlock = button.colors;
		}
	}

	public void Start() {
		provider.onChange += ProviderOnChange;
		ProviderOnChange();
	}

	void ProviderOnChange() {
		UpdateColor();
	}

	void UpdateColor() {
		if (renderer != null) {
			renderer.material.color = provider.Value;
		}
		if (button != null) {
			var newColorBlock = buttonColorBlock;
			newColorBlock.highlightedColor *= provider.Value;
			newColorBlock.disabledColor *= provider.Value;
			newColorBlock.normalColor *= provider.Value;
			newColorBlock.pressedColor *= provider.Value;
			newColorBlock.selectedColor *= provider.Value;
			button.colors = newColorBlock;
		}
	}

	private void Update() {
		if (checkOnUpdate) {
			UpdateColor();
		}
	}
}
