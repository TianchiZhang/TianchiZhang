using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCell : MonoBehaviour
{
    public CharacterCellData CellData;
    private Image _image = null;
    private Color _transparencyColor = new Color(1,1,1,0.3f);
    private Color _opaqueColor = Color.white;
    public RectTransform MaskTransform;
    public Material OutLineMat;
    public Material DissolveMat;
    private Material _defaultMat;
    private Shadow _shadow;
    public Action<CharacterCell> CellClicked;
    public int CellID;
    private Vector3 _defaultLocalScale = Vector3.one * 0.95f;
    public CharacterCellState State;
    private Vector2 _bgGoDir;
    private Vector2 _initMaskSize;
    private WaitForSeconds _sleep;
    private Transform _childTransform;
    private Vector3 _initMaskPosition;
    public void InitItem()
    {
        _sleep = new WaitForSeconds(0.02f);
        _image = transform.GetChild(0).GetComponent<Image>();
        string imageName = CellData.image.url.Split('/')[CellData.image.url.Split('/').Length - 2];
        _image.sprite = Resources.Load<Sprite>("res/" + imageName);
        _image.color = _transparencyColor;    
        _defaultMat = transform.GetChild(0).GetComponent<Material>();
        _shadow = transform.GetChild(0).GetComponent<Shadow>();
        _childTransform = transform.GetChild(0).transform;
        _childTransform.localScale = _defaultLocalScale;
        CellID = int.Parse(CellData.id);
        transform.GetComponent<Button>().onClick.AddListener(ButtonClicked);
        transform.GetComponent<Image>().material = new Material(DissolveMat);
        DissolveMat = transform.GetComponent<Image>().material;       
        State = CharacterCellState.Disable;
    }

    public IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        MaskTransform.position = transform.position;
        _initMaskSize = new Vector2(MaskTransform.rect.width, MaskTransform.rect.height);
        _initMaskPosition = MaskTransform.localPosition;
    }

    public void SetTransparent() {
        _image.color = _transparencyColor;
    }
    public void Setopaque()
    {
        _image.color = _opaqueColor;
    }

    private void ButtonClicked()
    {
        if (CellClicked != null)
        {
            CellClicked(this);
        }
    }

    public void ActiveMaskPath()
    {
        State = CharacterCellState.Selected;
        MaskTransform.position = transform.position;
        MaskTransform.gameObject.SetActive(true);
        DissolveMat.SetFloat("_ThresholdAlpha", 1f);
        transform.GetChild(0).GetComponent<Image>().material = OutLineMat;
        Setopaque();
    }

    public void SetEnableState()
    {
        State = CharacterCellState.Enable;
        Setopaque();
        _shadow.enabled = true;
        _shadow.effectDistance = new Vector2(0, -10);
        transform.GetChild(0).transform.localScale = Vector3.one;
        transform.GetComponent<Button>().enabled = true;
    }

    public void GoSelectedState()
    {
        StartCoroutine(GoSelectedCro());
    }

    private IEnumerator GoSelectedCro()
    {
        State = CharacterCellState.Selected;
        transform.GetChild(0).GetComponent<Image>().material = OutLineMat;
        _shadow.enabled = false;
        transform.GetComponent <Button>().enabled = false;
        while (DissolveMat.GetFloat("_ThresholdAlpha") < 1)
        {
            yield return _sleep;
            DissolveMat.SetFloat("_ThresholdAlpha", DissolveMat.GetFloat("_ThresholdAlpha") + 0.02f);
        }
    }

    public void ControlBgGoNext(Vector2 direction)
    {
        _bgGoDir = direction;
        StartCoroutine(ControlBgGoCro(direction));
    }

    public IEnumerator ControlBgGoCro(Vector2 direction)
    {
        MaskTransform.gameObject.SetActive(true);
        if (direction.x != 0)
        {
            MaskTransform.pivot = new Vector2(direction.x > 0 ? 0 : 1, 0.5f);
            MaskTransform.localPosition = new Vector3(_initMaskPosition.x - direction.x * _initMaskSize.x/2, _initMaskPosition.y, _initMaskPosition.z);
        }
        if (direction.y != 0)
        {
            MaskTransform.pivot = new Vector2(0.5f, direction.y > 0 ? 1 : 0);
            MaskTransform.localPosition = new Vector3(_initMaskPosition.x, _initMaskPosition.y + direction.y * _initMaskSize.y / 2, _initMaskPosition.z);
        }
        while (MaskTransform.rect.width + MaskTransform.rect.height < _initMaskSize.x + _initMaskSize.y * 2)
        {
            yield return _sleep;
            if (direction.x != 0)
            {               
                MaskTransform.sizeDelta += Vector2.right * _initMaskSize.x * 0.16f;
                if (MaskTransform.sizeDelta.x > _initMaskSize.x * 2)
                {
                    MaskTransform.sizeDelta = new Vector2(_initMaskSize.x *2, _initMaskSize.y);
                }
            }
            else if (direction.y != 0)
            {
                MaskTransform.sizeDelta += Vector2.up * _initMaskSize.y * 0.16f;
                if (MaskTransform.sizeDelta.y > _initMaskSize.y * 2)
                {
                    MaskTransform.sizeDelta = new Vector2(_initMaskSize.x, _initMaskSize.y * 2);
                }
            }
        }
    }

    public void ControlBgGoBack()
    {
        StartCoroutine(ControlBgGoBackCro());
    }

    public IEnumerator ControlBgGoBackCro()
    {
        while (MaskTransform.rect.width + MaskTransform.rect.height > _initMaskSize.x + _initMaskSize.y)
        {
            yield return _sleep;
            if (_bgGoDir.x != 0)
            {
                MaskTransform.sizeDelta -= Vector2.right * _initMaskSize.x * 0.16f;
                if (MaskTransform.sizeDelta.x < _initMaskSize.x)
                {
                    MaskTransform.sizeDelta = new Vector2(_initMaskSize.x, _initMaskSize.y);
                }
            }
            if (_bgGoDir.y != 0)
            {
                MaskTransform.sizeDelta -= Vector2.up * _initMaskSize.y * 0.16f;
                if (MaskTransform.sizeDelta.y < _initMaskSize.y)
                {
                    MaskTransform.sizeDelta = new Vector2(_initMaskSize.x, _initMaskSize.y);
                }
            }
        }
    }
    public void GoDisable()
    {
        StartCoroutine(GoDisableCro());
    }

    private IEnumerator GoDisableCro()
    {
        State = CharacterCellState.Disable;
        transform.GetComponent<Button>().enabled = false;
        _shadow.enabled = false;
        _image.material = _defaultMat;
        while (_childTransform.localScale.x > 0.95f)
        {
            yield return _sleep;
            _childTransform.localScale -= Vector3.one * 0.003f;
            _image.color = new Color(1,1,1, _image.color.a - 0.05f);
            if (_image.color.a <= _transparencyColor.a)
            {
                _image.color = new Color(1, 1, 1, _transparencyColor.a);
            }
        }      
    }

    public void GoEnable()
    {
        if (State == CharacterCellState.Disable)
        {
            StartCoroutine(GoEnableCro());
        }
        else if (State == CharacterCellState.Selected)
        {
            StartCoroutine(GoEnableFromSelectCro());
        }        
    }

    public IEnumerator GoEnableFromSelectCro() {
        State = CharacterCellState.Enable;
        _image.material = _defaultMat;
        _shadow.enabled = true;
        transform.GetComponent<Button>().enabled = true;
        MaskTransform.gameObject.SetActive(false);
        while (DissolveMat.GetFloat("_ThresholdAlpha") > 0)
        {
            yield return _sleep;
            DissolveMat.SetFloat("_ThresholdAlpha", DissolveMat.GetFloat("_ThresholdAlpha") - 0.02f);
        }
        DissolveMat.SetFloat("_ThresholdAlpha", 0);
    }

    public IEnumerator GoEnableCro()
    {
        State = CharacterCellState.Enable;
        transform.GetComponent<Button>().enabled = true;
        _shadow.enabled = true;
        while (_childTransform.localScale.x < 1f)
        {
            yield return _sleep;
            _childTransform.localScale += Vector3.one * 0.003f;
            _image.color = new Color(1, 1, 1, _image.color.a + 0.05f);
            if (_image.color.a >= _opaqueColor.a)
            {
                _image.color = new Color(1, 1, 1, _opaqueColor.a);
            }
        }
    }

    public void Shake()
    {
        StartCoroutine(ShakeCro());
    }

    private IEnumerator ShakeCro()
    {
        float rotate = 0;
        while (rotate < 4 * Mathf.PI) { 
            yield return _sleep;
            rotate += 0.5f;
            _image.transform.rotation = Quaternion.Euler(0, 0, 15*Mathf.Sin(rotate));
        }
        _image.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}

