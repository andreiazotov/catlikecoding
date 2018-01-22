﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    /*
     * Mesh is a construct used by the graphics hardware to draw complex stuff.
     * It's a 3D object that's either imported into Unity or generated by code.
     * A mesh contains at least a collection of points in 3D space plus a set of
     * triangles the most basic 2D shapes defined by these points.
     * The triangles constitute the surface of whatever the mesh represents.
    */
    public Mesh[] meshes;

    /* Materials are used to define the visual properties of objects.
     * They can range from very simple, like a constant color, to very complex.
     * Materials consist of a shader and whatever data the shader needs.
     * Shaders are basically scripts that tell the graphics card how an object's
     * polygons should be drawn.
    */
    public Material material;

    public int maxDepth;
    public float childScale;
    public float spawnProbability;
    public float maxRotationSpeed;
    public float maxTwist;

    private float _rotationSpeed;
    private int _depth;
    private Material[,] _materials;

    private static Vector3[] s_childDirections =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] s_childOrientation =
    {
        Quaternion.identity,
        Quaternion.Euler(0.0f, 0.0f, -90.0f),
        Quaternion.Euler(0.0f, 0.0f, 90.0f),
        Quaternion.Euler(90.0f, 0.0f, 0.0f),
        Quaternion.Euler(-90.0f, 0.0f, 0.0f)
    };

    private void Start()
    {
        if (this._materials == null)
        {
            this.InitializeMaterials();
        }
        this._rotationSpeed = Random.Range(-this.maxRotationSpeed, this.maxRotationSpeed);
        this.transform.Rotate(Random.Range(-this.maxTwist, this.maxTwist), 0.0f, 0.0f);
        this.gameObject.AddComponent<MeshFilter>().mesh = this.meshes[Random.Range(0, this.meshes.Length)];
        this.gameObject.AddComponent<MeshRenderer>().material = this._materials[this._depth, Random.Range(0, 2)];
        if (this._depth < this.maxDepth)
        {
            this.StartCoroutine(this.CreateChildren());
        }
    }

    private void InitializeMaterials()
    {
        this._materials = new Material[this.maxDepth + 1, 2];
        for (int i = 0; i <= this.maxDepth; i++)
        {
            float t = i / (this.maxDepth - 1.0f);
            t *= t;
            this._materials[i, 0] = new Material(this.material)
            {
                color = Color.Lerp(Color.white, Color.yellow, t)
            };
            this._materials[i, 1] = new Material(this.material)
            {
                color = Color.Lerp(Color.white, Color.cyan, t)
            };
        }
        this._materials[this.maxDepth, 0].color = Color.magenta;
        this._materials[this.maxDepth, 1].color = Color.red;
    }

    private void Initialize(Fractal parent, int childIndex)
    {
        this.meshes = parent.meshes;
        this.material = parent.material;
        this.maxDepth = parent.maxDepth;
        this.spawnProbability = parent.spawnProbability;
        this.maxRotationSpeed = parent.maxRotationSpeed;
        this.maxTwist = parent.maxTwist;
        this._depth = parent._depth + 1;
        this._materials = parent._materials;
        this.childScale = parent.childScale;
        this.transform.parent = parent.transform;
        this.transform.localScale = Vector3.one * this.childScale;
        this.transform.localPosition = s_childDirections[childIndex] * (0.5f + 0.5f * this.childScale);
        this.transform.localRotation = s_childOrientation[childIndex];
    }

    private IEnumerator CreateChildren()
    {
        /*
         * First the new game object is created. Then a new Fractal component is
         * created and added to it. At this point its Awake and OnEnable methods
         * would be invoked, if they had existed. Then the AddComponent method
         * finishes. Directly after that we invoke Initialize. The call to Start
         * won't happen until the next frame.
        */
        for (int i = 0; i < s_childDirections.Length; i++)
        {
            if (this.spawnProbability > Random.value)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("FractalChild").AddComponent<Fractal>().Initialize(this, i);
            }
        }
    }

    private void Update()
    {
        this.transform.Rotate(0.0f, this._rotationSpeed * Time.deltaTime, 0.0f);
    }
}