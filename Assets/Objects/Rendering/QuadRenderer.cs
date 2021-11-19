using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadRenderer : MonoBehaviour
{
    public Texture texture
    {
        get => _texture;
        set {
            _texture = value;
            UpdateTexture();
        }
    }

    public bool test;

    private MaterialPropertyBlock _propBlock;

    [SerializeField] private Texture _texture;
    
    private void Start() {
        _material = GetComponent<MeshRenderer>().material;
        _propBlock = new MaterialPropertyBlock();
        UpdateTexture();
    }

    private void UpdateTexture() {
        _material.SetTexture("_BaseMap", _texture);
        //_renderer.GetPropertyBlock(_propBlock);
        //_propBlock.SetTexture("_BaseMap", _texture);
    }

    private void OnValidate() {
        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_BaseMap", _texture);
    }

    private void OnDestroy() {
        Destroy(_material);
    }
    
    private MeshRenderer _renderer;
    private Material _material;
}
