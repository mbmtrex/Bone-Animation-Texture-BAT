using UnityEngine;
using System.IO;

public class BATRecorder : MonoBehaviour
{
    private Animator _animator;
    private SkinnedMeshRenderer _meshRenderer;
    private int _frameNumber;
    private StreamWriter _writer;
    private string _header;
    private float _localTime;
    private string _filePathCSV;
    
    private Texture2D _posTexture;
    private Texture2D _rotTexture;
    private Texture2D _scaleTexture;
    
    [SerializeField] private string _filePath;
    [SerializeField] private writeMode _writeMode; 

    [HideInInspector] public AnimationClip[] _animationClip;
    [HideInInspector] public int _textureWidth;
    [HideInInspector] public int _textureHeight;
    [HideInInspector] public int _clipCount;
    

    [ContextMenu("BAT_Recorder")]
    public void Initalize()
    {
        _animationClip=null;
        _frameNumber = 0;
        _clipCount = 0;
        _textureWidth = 0;
        _textureHeight = 0;
        _animator = null;
        _meshRenderer = null;
        _posTexture = null;
        _rotTexture = null;
        _scaleTexture = null;
        
        _animator = GetComponent<Animator>();
        _animationClip = _animator.runtimeAnimatorController.animationClips;
        _clipCount=_animationClip.Length;
        _frameNumber = 0;
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
       
        if(_filePath == null)
            _filePath = Application.dataPath + "/BAT_";
        
        for (int i = 0; i < _animationClip.Length; i++)
        {
            _textureHeight += (int)(_animationClip[i].length * _animationClip[i].frameRate);
        }
        
        _textureWidth = _meshRenderer.bones.Length;
    }
    [ContextMenu("BAT_Recorder")]
    public void RecordAnim()
    {
        if (_writeMode != writeMode.CSV)
        {
            _posTexture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBAFloat, false);
            _rotTexture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBAFloat, false);
            _scaleTexture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBAFloat, false);
        }

        if(_writeMode==writeMode.CSV)
        {
            _filePathCSV = _filePath + "CSV.csv";
            _writer = new StreamWriter(_filePathCSV);
            _header = "Frame Number,";
            for (int i = 0; i < _meshRenderer.bones.Length; i++)
            {
                _header += "Bone " + i + " X,Bone " + i + " Y,Bone " + i + " Z,";
            }
            _writer.WriteLine(_header);
        }
        
        if (_clipCount == 0)
            return;
        
        for(int i=0;i<_animationClip.Length;i++)
        {
            _clipCount--;
            var thisClipFrameRate=0;
            for (int j = 0; j < _animationClip[i].frameRate*_animationClip[i].length; j++)
            {
                _animationClip[i].SampleAnimation(gameObject, thisClipFrameRate / _animationClip[i].frameRate);
                thisClipFrameRate += 1;
                _frameNumber += 1;
                
                if(_writeMode==writeMode.Texture)
                {
                    WriteFullMatrix(_frameNumber);
                }
                else if(_writeMode==writeMode.CSV)
                {
                    WriteInCSV();
                }
            }
        }
        if(_writeMode==writeMode.CSV)
            _writer.Close();
        else
        {
            System.IO.File.WriteAllBytes(_filePath + "Position.exr", _posTexture.EncodeToEXR());
            System.IO.File.WriteAllBytes(_filePath + "Rotation.exr", _rotTexture.EncodeToEXR());
            System.IO.File.WriteAllBytes(_filePath + "Scale.exr", _scaleTexture.EncodeToEXR());
        }
    }
    void WriteFullMatrix(int frameNumber)
    {
        for (int k = 0; k < _meshRenderer.bones.Length; k++)
        {
            var tempMatrix = _meshRenderer.bones[k].localToWorldMatrix * _meshRenderer.sharedMesh.bindposes[k];
            
            _posTexture.SetPixel(k, frameNumber, new Color(tempMatrix.m00, tempMatrix.m01, tempMatrix.m02, tempMatrix.m03));
            _rotTexture.SetPixel(k, frameNumber, new Color(tempMatrix.m10, tempMatrix.m11, tempMatrix.m12, tempMatrix.m13));
            _scaleTexture.SetPixel(k, frameNumber, new Color(tempMatrix.m20, tempMatrix.m21, tempMatrix.m22, tempMatrix.m23));
        }
    }
    void WriteInCSV()
    {
        string line = _frameNumber + " , ";
        for (int i = 0; i < _meshRenderer.bones.Length; i++)
        {
            Vector3 position = _meshRenderer.bones[i].position;
            line += position.x + "," + position.y + "," + position.z + ",";
        }
        _writer.WriteLine(line);
    }
    private enum writeMode
    {
        Texture,CSV
    }
}

