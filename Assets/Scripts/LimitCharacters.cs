﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitCharacters : MonoBehaviour {

    public InputField input;

	// Use this for initialization
	void Start () {
        input.characterLimit = 36;
	}
}
