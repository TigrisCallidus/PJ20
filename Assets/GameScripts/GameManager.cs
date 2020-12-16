using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour {


    public WebGLImageProvider ImageProvider;
    public TerrainGeneratorColormix Generator;

    public GameObject StartUI;
    public GameObject Loading;
    public GameObject ConvertingTexture;
    public GameObject Generate2DData;
    public GameObject Generate3DData;
    public GameObject GeneratingBlur;
    public GameObject GeneratingMesh;
    public GameObject ApplyColor;
    public GameObject ChoosingStartLocation;
    public GameObject Ready;

    public GameObject Camera;
    public GameObject Camera2;
    public RigidbodyFirstPersonControllerClimbing Climber;

    public GameObject[] Triggers;


    public Texture2D[] ExampleTextures;

    public Image Fade;

    public float FadeDuration = 2f;

    public GameObject Finished;
    public Text Duration;

    public GameObject Menu;


    int triggercount = 0;
    bool menuVisible = false;

    float startTime = 0;

    public void PlayDefaultLevel(int textureChosen) {
        ApplyTexture(textureChosen);
    }

    public void UploadOwnImage() {
        ImageProvider.ShowUploadDialog(ApplyTexture);
        //Loading?.SetActive(true);
        //ConvertingTexture?.SetActive(true);
    }


    IEnumerator generateSequence() {
        StartUI?.SetActive(false);
        Loading?.SetActive(true);
        ConvertingTexture?.SetActive(false);
        GeneratingBlur?.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        Generator.ApplyBlur();
        Generator.TextureToBlur = null;
        GC.Collect();

        yield return new WaitForSeconds(0.2f);
        GeneratingBlur?.SetActive(false);
        Generate2DData?.SetActive(true);
        yield return null;
        Generator.Generate2DData();

        Destroy(Generator.InputTexture);
        Generator.InputTexture = null;
        GC.Collect();

        yield return new WaitForSeconds(0.2f);
        Generate2DData?.SetActive(false);
        Generate3DData?.SetActive(true);
        yield return null;
        Generator.Generate3DDataFrom2DData();

        //Generator.Data2D = null;
        GC.Collect();

        yield return new WaitForSeconds(0.2f);
        Generate3DData?.SetActive(false);
        GeneratingMesh?.SetActive(true);
        yield return null;
        Generator.GenerateMesh(false);
        Physics.gravity = SlowFall;
        SetCamera();
        SetTriggers();

        Generator.Data2D = null;
        Generator.Data3D = null;
        GC.Collect();

        yield return new WaitForSeconds(0.2f);
        GeneratingMesh?.SetActive(false);
        ApplyColor?.SetActive(true);
        yield return null;
        Generator.ColorMesh();

        yield return new WaitForSeconds(0.2f);
        ApplyColor?.SetActive(false);
        ChoosingStartLocation?.SetActive(true);
        yield return null;
        //SetCamera();

        yield return new WaitForSeconds(3f);
        ChoosingStartLocation?.SetActive(false);
        //Ready?.SetActive(true);
        Loading?.SetActive(false);
        Physics.gravity = NormalFall;
        startTime = Time.time;
    }

    public void SetCamera() {
        /*
        int x = Generator.Data3D.X / 2;
        int y = Generator.Data3D.Y / 2;
        int z = Generator.Data3D.Z / 2;

        int height = 0;
        for (int i = 0; i < z; i++) {
            if (Generator.Data3D[x,y,z].Value>=1) {
                height++;
            } else {
                break;
            }
        }
        Debug.Log(height);

        Camera.transform.position = new Vector3(0, height+1.5f, 0);
        */

        Camera.SetActive(false);
        Camera2.transform.position = new Vector3(0, 50, 0);
        Camera2.SetActive(true);

    }

    void SetTriggers() {

        int x = Generator.Data3D.X / 4;
        int y = Generator.Data3D.Y / 4;
        //int z = Generator.Data3D.Z / 2;

        for (int i = 0; i < Triggers.Length; i++) {
            Triggers[i]?.SetActive(true);
            if (i == 0 && Triggers[i]!=null) {
                Triggers[i].transform.position = new Vector3(x, 50, y);
            } else if (i == 1 && Triggers[i] != null) {
                Triggers[i].transform.position = new Vector3(x, 50, -y);
            } else if (i == 2 && Triggers[i] != null) {
                Triggers[i].transform.position = new Vector3(-x, 50, y);
            } else if (i == 3 && Triggers[i] != null) {
                Triggers[i].transform.position = new Vector3(-x, 50, -y);
            }
        }
    }

    public void ApplyTexture(int textureNumber) {
        ApplyTexture(ExampleTextures[textureNumber]);
    }

    public void ApplyTexture(Texture2D texture) {
        Generator.TextureToBlur = texture;
        StartCoroutine(generateSequence());
    }


    // Start is called before the first frame update
    void Start() {
        Application.targetFrameRate = 30;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ShowMenu();
        }
    }

    bool goingOut = false;
    public void GoOut() {
        if (!goingOut) {
            StartCoroutine(goOut());
        }
    }

    IEnumerator goOut() {

        goingOut = true;

        float progress = 0;

        while (progress < 1) {
            progress += Time.deltaTime / FadeDuration;
            Fade.color = new Color(0, 0, 0, progress);
            yield return null;
        }
        Fade.color = new Color(0, 0, 0, 1);
        SetCamera();
        yield return new WaitForSeconds(2);
        while (progress > 0) {
            progress -= Time.deltaTime / FadeDuration;
            Fade.color = new Color(0, 0, 0, progress);
            yield return null;
        }
        Fade.color = new Color(0, 0, 0, 0);


        yield return null;
        goingOut = false;
    }

    public void TriggerChange() {
        triggercount++;
        Debug.Log("Color changed");
        if (triggercount >= Triggers.Length) {
            ShowEnd();
        }
    }

    public void ShowEnd() {
        float totalTime = Time.time - startTime;
        int totalSeconds = Mathf.RoundToInt(totalTime);
        Duration.text = totalSeconds.ToString() + " seconds";
        Finished?.SetActive(true);
        Climber.PauseMouselock(true);
    }

    public  void ExploreFurther() {
        Finished?.SetActive(false);
        Climber.PauseMouselock(false);
    }

    public void ShowMenu() {
        menuVisible = !menuVisible;
        Menu?.SetActive(menuVisible);
        Climber.PauseMouselock(menuVisible);
    }
    public void CloseGame() {
        Application.Quit();
    }

    public static Vector3 SlowFall = new Vector3(0, -1.0f, 0);
    public static Vector3 NormalFall = new Vector3(0, -9.81f, 0);

}
