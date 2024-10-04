using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Animations;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Array = System.Array;
public class SpriteToAnimation : EditorWindow
{
    private Texture2D[] selectedTextures;
    private AnimationClip[] selectedAnimations;
    private Vector2 pivot;
    private string savePath;

    private AnimatorController animatorController;


    string animationStateName = "State";
    string animationName = "Animation Name";
    bool isLooping;
    float factor = 1;
    bool destroyPrevious = true;

    int frameCount = 1;
    int frameWidth = 256;
    int frameHeight = 256;
    // Create a Regex
    Regex animationFilenamePattern = new(@"^(\w+)_(\d+)x(\d+)_(\d+)_(N|S|W|E|NE|NW|SE|SW)$");
    public struct AutomaticAnimationSetupRecipe
    {
        public string animationName;
        public string animationState;
        public float factor;
        public bool destroyPrevious;

    }


    public struct AnimationInfo
    {
        public string animationName;
        public string directionName;
        public int frameWidth;
        public int frameHeight;
        public int frameCount;
        public bool valid;
    }


    AnimationInfo ParseAnimationFilename(string animationFilename)
    {
        var result = animationFilenamePattern.Match(animationFilename);
        if (!result.Success) return default;
        return new()
        {
            valid = true,
            animationName = result.Groups[1].Value,
            frameWidth = int.Parse(result.Groups[2].Value),
            frameHeight = int.Parse(result.Groups[3].Value),
            frameCount = int.Parse(result.Groups[4].Value),
            directionName = result.Groups[5].Value,
        };
    }

    AutomaticAnimationSetupRecipe[] automaticAnimationSetupRecipes = new AutomaticAnimationSetupRecipe[]{
        new(){
            animationName = "idle",
            factor = 0.25f,
            animationState = "Movement",
            destroyPrevious = true,
        },
        new(){
            animationName = "walk",
            factor = 0.5f,
            animationState = "Movement",
            destroyPrevious = false,
        },
        new(){
            animationName = "run",
            factor = 1f,
            animationState = "Movement",
            destroyPrevious = false,
        },
        new(){
            animationName = "attack",
            factor = 0.25f,
            animationState = "Attack",
            destroyPrevious = true,
        },
        new(){
            animationName = "hit",
            factor = 0.25f,
            animationState = "Hit",
            destroyPrevious = true,
        },
         new(){
            animationName = "death",
            factor = 0.25f,
            animationState = "Death",
            destroyPrevious = true,
        },
    };

    [MenuItem("Tools/Sprite Sheet to Animation")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SpriteToAnimation));
    }

    private void OnSelectionChange()
    {
        selectedTextures = null;
        selectedAnimations = null;
        if (Selection.activeObject is Texture2D && Selection.assetGUIDs.Length > 0)
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is not Texture2D) return;
            }
            selectedTextures = new Texture2D[Selection.objects.Length];
            System.Array.Copy(Selection.objects, selectedTextures, Selection.objects.Length);
        }
        else if (Selection.activeObject is AnimationClip && Selection.assetGUIDs.Length > 0)
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is not AnimationClip) return;
            }
            selectedAnimations = new AnimationClip[Selection.objects.Length];
            System.Array.Copy(Selection.objects, selectedAnimations, Selection.objects.Length);
        }
    }

    private void OnGUI()
    {




        GUILayout.Label("Sprite Sheet to Animation", EditorStyles.boldLabel);
        animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), false);
        pivot = EditorGUILayout.Vector2Field("Pivot", pivot);
        frameCount = EditorGUILayout.IntField("Frame Count", frameCount);
        frameWidth = EditorGUILayout.IntField("Frame Width", frameWidth);
        frameHeight = EditorGUILayout.IntField("Frame Height", frameHeight);
        isLooping = EditorGUILayout.Toggle("Is Loop", isLooping);
        GUI.enabled = selectedTextures != null;
        {
            if (GUILayout.Button("Slice Image"))
            {
                foreach (var texture in selectedTextures)
                {
                    SliceImage(texture);
                }
            }

            if (GUILayout.Button("Create Animation"))
            {

                foreach (var texture in selectedTextures)
                {
                    CreateAnimationFromSpriteSheet(texture);
                }
            }

            if (selectedTextures != null && GUILayout.Button($"Auto Slice Selection ({selectedTextures.Length})"))
            {
                AutoSliceSelection();
            }
            if (selectedTextures != null && GUILayout.Button($"Auto Animation Clips From Selection ({selectedTextures.Length})"))
            {
                AutoAnimationClips();
            }

        }
        GUI.enabled = animatorController != null;
        GUILayout.BeginVertical("box");
        GUILayout.Label("Blend Trees");
        animatorController = (AnimatorController)EditorGUILayout.ObjectField("Controller", animatorController, typeof(AnimatorController), false);
        animationName = EditorGUILayout.TextField("Animation", animationName);
        animationStateName = EditorGUILayout.TextField("Animation State", animationStateName);
        factor = EditorGUILayout.FloatField("Factor", factor);
        destroyPrevious = EditorGUILayout.Toggle("Destroy Previous Tree", destroyPrevious);
        {
            if (GUILayout.Button("Setup Animations"))
            {
                SetupAnimations();
            }
            GUI.enabled = selectedAnimations != null;
            if (GUILayout.Button("Run automatic setup Animations"))
            {
                AutomaticSetupAnimations();
            }

        }
        GUILayout.EndVertical();
    }



    BlendTree GetBlendTree(string name, bool destroyPrevious)
    {
        var rootStateMachine = animatorController.layers[0].stateMachine;
        var state = rootStateMachine.states.FirstOrDefault(v => v.state.name == name).state;
        if (!state)
        {
            state = rootStateMachine.AddState(name);
        }
        if (destroyPrevious || state.motion is not BlendTree)
        {

            var tree = new BlendTree();
            state.motion = tree;
        }
        return state.motion as BlendTree;
    }

    public struct IsoAnim
    {
        public readonly Vector2 direction;
        public readonly string directionName;

        public IsoAnim(Vector2 direction, string directionName)
        {
            this.direction = direction;
            this.directionName = directionName;
        }
    }
    public static readonly IsoAnim[] directions = {
        new IsoAnim(new Vector2(1, 1).normalized, "W"),
        new IsoAnim(new Vector2(-1, 1).normalized, "S"),
        new IsoAnim(new Vector2(1, -1).normalized, "N"),
        new IsoAnim(new Vector2(-1, -1).normalized, "E"),
        new IsoAnim(new Vector2(1, 0).normalized, "NW"),
        new IsoAnim(new Vector2(-1, 0).normalized, "SE"),
        new IsoAnim(new Vector2(0, 1).normalized, "SW"),
        new IsoAnim(new Vector2(0, -1).normalized, "NE"),
    };

    void AddAnimationAllDirections(BlendTree tree, AnimationClip animation, string direction, float factor)
    {
        tree.blendParameter = "Horizontal";
        tree.blendParameterY = "Vertical";
        tree.blendType = BlendTreeType.FreeformCartesian2D;
        foreach (var dir in directions)
        {
            if (dir.directionName == direction)
            {
                tree.AddChild(animation, dir.direction * factor);
                break;
            }
        }
    }

    void AddAnimationAllDirections(BlendTree tree, string animation, float factor)
    {
        var directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(animatorController));
        tree.blendParameter = "Horizontal";
        tree.blendParameterY = "Vertical";
        tree.blendType = BlendTreeType.FreeformCartesian2D;
        foreach (var dir in directions)
        {
            var path = Path.Combine(directory, "Animations", $"{animation}_{dir.directionName}.anim");
            var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            tree.AddChild(anim, dir.direction * factor);
        }
    }

    void AutoAnimationClips()
    {
        if (selectedTextures == null) return;

    }
    private void SetupAnimations()
    {
        var controllerPath = AssetDatabase.GetAssetPath(animatorController);
        var rootStateMachine = animatorController.layers[0].stateMachine;
        if (animatorController.parameters.FirstOrDefault(p => p.name == "Horizontal") == null)
        {
            animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        }
        if (animatorController.parameters.FirstOrDefault(p => p.name == "Vertical") == null)
        {
            animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        }
        // var stateMachineA = rootStateMachine.AddState("smA");
        // var tree = new BlendTree();
        var movementTree = GetBlendTree(animationStateName, destroyPrevious);
        AddAnimationAllDirections(movementTree, animationName, factor);
        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssetIfDirty(animatorController);
    }

    private void AutomaticSetupAnimations()
    {
        if (selectedAnimations == null) return;
        var controllerPath = AssetDatabase.GetAssetPath(animatorController);
        var rootStateMachine = animatorController.layers[0].stateMachine;
        if (animatorController.parameters.FirstOrDefault(p => p.name == "Horizontal") == null)
        {
            animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        }
        if (animatorController.parameters.FirstOrDefault(p => p.name == "Vertical") == null)
        {
            animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        }
        // var stateMachineA = rootStateMachine.AddState("smA");
        // var tree = new BlendTree();
        foreach (var recipe in automaticAnimationSetupRecipes)
        {
            var animations = Array.FindAll(selectedAnimations, a => a.name.StartsWith(recipe.animationName));
            if (animations.Length == 0) continue;
            var movementTree = GetBlendTree(recipe.animationState, recipe.destroyPrevious);
            foreach (var animation in animations)
            {
                var clipInfo = ParseAnimationFilename(animation.name);
                Debug.Log($"{clipInfo.animationName} {clipInfo.directionName}");
                AddAnimationAllDirections(movementTree, animation, clipInfo.directionName, recipe.factor);
            }

        }
        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssetIfDirty(animatorController);
    }


    void SaveImporter(TextureImporter importer)
    {
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }


    private void AutoSliceSelection()
    {

        if (selectedTextures == null) return;
        var toBeSaved = new List<TextureImporter>();
        foreach (var texture in selectedTextures)
        {
            var info = ParseAnimationFilename(texture.name);
            if (!info.valid)
            {
                Debug.LogError($"Failed to slice ${texture.name}! make sure that the name structure is correct");
                continue;
            }
            string assetPath = AssetDatabase.GetAssetPath(texture);
            // Set texture type to Sprite (2D and UI)
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.filterMode = FilterMode.Point;
            // Slice sprite sheet into 128x128 frames
            int columns = texture.width / info.frameWidth;
            int rows = texture.height / info.frameHeight;
            var spriteSheet = new SpriteMetaData[info.frameCount];
            var index = 0;
            for (int y = rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < columns && index < spriteSheet.Length; x++, index++)
                {
                    SpriteMetaData frameMeta = new SpriteMetaData();
                    frameMeta.pivot = new Vector2(0.5f, 0.5f);
                    frameMeta.name = texture.name + "_" + index;
                    frameMeta.rect = new Rect(x * info.frameWidth, y * info.frameHeight, info.frameWidth, info.frameHeight);
                    frameMeta.alignment = (int)SpriteAlignment.Custom;
                    frameMeta.pivot = pivot / new Vector2(info.frameWidth, info.frameHeight);
                    spriteSheet[index] = frameMeta;

                }
            }
            textureImporter.spritesheet = spriteSheet;
            toBeSaved.Add(textureImporter);
            Debug.Log($"Sliced {info.animationName}: frames={spriteSheet.Length}");
        }

        foreach (var s in toBeSaved)
        {
            SaveImporter(s);
        }
    }

    private void SliceImage(Texture2D sourceImage)
    {
        // Get image path
        string assetPath = AssetDatabase.GetAssetPath(sourceImage);
        // Set texture type to Sprite (2D and UI)
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.filterMode = FilterMode.Point;
        // Slice sprite sheet into 128x128 frames
        int columns = sourceImage.width / frameWidth;
        int rows = sourceImage.height / frameHeight;
        var spriteSheet = new SpriteMetaData[frameCount];
        var index = 0;
        for (int y = rows - 1; y >= 0; y--)
        {
            for (int x = 0; x < columns && index < spriteSheet.Length; x++, index++)
            {
                SpriteMetaData frameMeta = new SpriteMetaData();
                frameMeta.pivot = new Vector2(0.5f, 0.5f);
                frameMeta.name = sourceImage.name + "_" + index;
                frameMeta.rect = new Rect(x * frameWidth, y * frameHeight, frameWidth, frameHeight);
                frameMeta.alignment = (int)SpriteAlignment.Custom;
                frameMeta.pivot = pivot / new Vector2(frameWidth, frameHeight);
                spriteSheet[index] = frameMeta;

            }
        }
        textureImporter.spritesheet = spriteSheet;
        EditorUtility.SetDirty(textureImporter);
        textureImporter.SaveAndReimport();
        Debug.Log($"columns={columns} rows={rows} imgHeight={sourceImage.height} frameHeight={frameHeight} frames={spriteSheet.Length}");
    }

    private void CreateAnimationFromSpriteSheet(Texture2D sourceImage)
    {
        // Get image path
        string assetPath = AssetDatabase.GetAssetPath(sourceImage);
        string outputPath = Path.Join(Path.GetDirectoryName(assetPath), "Animations");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        var spriteFrames = System.Array.FindAll(sprites, s => s is Sprite);
        if (spriteFrames.Length == 0)
        {
            Debug.LogError("No sprites found after slicing. Check your image dimensions.");
            return;
        }

        // Create animation clip
        AnimationClip animClip = new AnimationClip();
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        var settings = AnimationUtility.GetAnimationClipSettings(animClip);
        settings.loopTime = isLooping;
        AnimationUtility.SetAnimationClipSettings(animClip, settings);
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteFrames.Length];
        float frameRate = 12f;  // You can change this to your desired frame rate

        for (int i = 0; i < keyFrames.Length; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / frameRate,
                value = spriteFrames[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, keyFrames);

        // Save animation clip
        string animClipPath = Path.Join(outputPath, sourceImage.name + ".anim");
        AssetDatabase.CreateAsset(animClip, animClipPath);
        EditorUtility.SetDirty(animClip);
        AssetDatabase.SaveAssetIfDirty(animClip);
        Debug.Log("Animation created at: " + animClipPath);
    }
}
