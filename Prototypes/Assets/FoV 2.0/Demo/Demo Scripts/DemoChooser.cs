using UnityEngine;
using System.Collections;

public class DemoChooser : MonoBehaviour {

	public void LoadEnemiesDemo() {

		Application.LoadLevel("DemoScene - Enemies");

	}

	public void LoadEnemiesMobileDemo() {
		
		Application.LoadLevel("DemoScene - Enemies Mobile");
		
	}

	public void LoadSecurityCamDemo() {

		Application.LoadLevel("DemoScene - Security Camera");

	}

	public void LoadTowerDemo() {

		Application.LoadLevel("DemoScene - Tower Rotating FoV");

	}

	public void LoadChooseDemoScene() {

		Application.LoadLevel("Demo Selection Scene");

	}

	public void Exit() {

		Application.Quit ();

	}

}