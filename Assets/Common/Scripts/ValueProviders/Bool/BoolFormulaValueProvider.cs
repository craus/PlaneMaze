using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Calculates bool value by formula
/// Formula sample: 
/// </summary>
public class BoolFormulaValueProvider : BoolValueProvider {
	/// <summary>
	/// Arguments to use for formula input
	/// They are indexed starting from 0
	/// </summary>
	public List<BoolValueProvider> arguments;

	/// <summary>
	/// Formula to calculate
	/// Sample: 0|-1&2
	/// This means "Zeroth argument OR BOTH (NOT first) and second arguments
	/// More samples: 
	/// 0&1&2
	/// 0
	/// </summary>
	public string formula;

	public bool Calculate(string formula) {
		if (formula.Contains('|')) {
			return formula.Split('|').Any(part => Calculate(part));
		}
		if (formula.Contains('&')) {
			return formula.Split('&').All(part => Calculate(part));
		}
		if (formula.Contains('-') || formula.Contains('!')) {
			return !Calculate(formula.Substring(1));
		}
		return arguments[int.Parse(formula)]?.Value ?? false;
	}

	public override bool Value {
		get {
			return Calculate(formula);
		}
	}
}
