using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterCellState
{ 
    Selected,
    Enable,
    Disable
}

public class GameController : MonoBehaviour
{
    public Tags GameTags;
    public List<int> Anwser = new List<int>();
    public List<CharacterCell> CurrentAnwser = new List<CharacterCell>();
    private int _characterCount = 0;
    public Transform CharacterParentTransform;
    public TMP_Text StepText;
    private Button _finishButton;
    private int _checkAnwserCount;
    public Button ResetButton;

    public void InitGame()
    {
        _characterCount = GameTags.cols * GameTags.rows;
        _finishButton = StepText.transform.parent.GetComponent<Button>();
        _finishButton.onClick.AddListener(OnFinishButtonClicked);
        ResetButton.onClick.AddListener(Reset);
    }

    public IEnumerator Start()
    {
        yield return null;
        CurrentAnwser.Add(CharacterParentTransform.GetChild(Anwser[0]).GetComponent<CharacterCell>());
        CurrentAnwser[0].ActiveMaskPath();
        List<CharacterCell> cells = GetNextCells(CurrentAnwser[0]);
        foreach (CharacterCell cell in cells)
        {
            cell.SetEnableState();
        }     
    }

    public void OnCellClicked(CharacterCell cell)
    {
        cell.GoSelectedState();
        CharacterCell lastCell = CurrentAnwser[CurrentAnwser.Count - 1];
        CurrentAnwser.Add(cell);
        lastCell.ControlBgGoNext(new Vector2(cell.CellData.colIndex - lastCell.CellData.colIndex, cell.CellData.rowIndex - lastCell.CellData.rowIndex));
        List<CharacterCell> cells = GetNextCells(lastCell);
        foreach (CharacterCell temp in cells)
        {
            if (temp.State == CharacterCellState.Enable)
                temp.GoDisable();
        }

        if (CurrentAnwser.Count < 5)
        {
            StepText.text = CurrentAnwser.Count + "/5";
        }
        else if (CurrentAnwser.Count == 5)
        {
            StepText.text = "->";
            StepText.transform.parent.GetComponent<Button>().interactable = true;
            return;
        }

        cells = GetNextCells(cell);
        foreach (CharacterCell temp in cells)
        {
            if (temp.State == CharacterCellState.Disable)
                temp.GoEnable();
        }
    }

    private List<CharacterCell> GetNextCells(CharacterCell currentCell)
    { 
        List<CharacterCell> cells = new List<CharacterCell>();
        CharacterCell cell = GetCharacterCell(currentCell.CellID + 1);
        if (cell != null)
        {
            if(cell.CellData.rowIndex == currentCell.CellData.rowIndex && !CurrentAnwser.Contains(cell))
                cells.Add(cell);
        }
        cell = GetCharacterCell(currentCell.CellID - 1);
        if (cell != null)
        {
            if (cell.CellData.rowIndex == currentCell.CellData.rowIndex && !CurrentAnwser.Contains(cell))
                cells.Add(cell);
        }
        cell = GetCharacterCell(currentCell.CellID + GameTags.cols);
        if (cell != null)
        {
            cells.Add(cell);
        }
        cell = GetCharacterCell(currentCell.CellID - GameTags.cols);
        if (cell != null)
        {
            cells.Add(cell);
        }
        return cells;
    }

    private CharacterCell GetCharacterCell(int cellID)
    {
        if (cellID < 0 || cellID > _characterCount -1)
            return null;
        return CharacterParentTransform.GetChild(cellID).GetComponent<CharacterCell>();
    }

    private void OnFinishButtonClicked()
    {
        _checkAnwserCount++;
        int backCount = 0;
        for (int i = 0; i < CurrentAnwser.Count; i++)
        {
            if (CurrentAnwser[i].CellID != Anwser[i])
            {
                backCount = CurrentAnwser.Count - i;
                break;
            }
        }

        if (_checkAnwserCount == 3)
        {
            StartCoroutine(ResetCro(backCount, ShowResultCro()));
            return;
        }
        
        if (backCount == 0)
        {
            StepText.text = "Success";
            StepText.transform.parent.GetComponent<Button>().interactable = false;
        }
        else
        {
            StepText.transform.parent.GetComponent<Button>().interactable = false;
            StartCoroutine(ResetCro(backCount, null));
        }
    }

    private IEnumerator ResetCro(int backCount, IEnumerator correctCro, bool shake = true)
    {
        if (shake)
        {
            for (int i = CurrentAnwser.Count - 1; i >= CurrentAnwser.Count - backCount; i--)
            {
                CurrentAnwser[i].Shake();
            }
        }       
        yield return new WaitForSeconds(1);
        int currentCount = CurrentAnwser.Count;
        for (int i = currentCount - 1; i >= currentCount - backCount; i--)
        {           
            CurrentAnwser[i].GoEnable();
            CurrentAnwser[i - 1].ControlBgGoBack();
            List<CharacterCell> cells = GetNextCells(CurrentAnwser[i]);
            CurrentAnwser.RemoveAt(i);
            StepText.text = CurrentAnwser.Count + "/5";
            foreach (CharacterCell cell in cells)
            {
                if (cell.State != CharacterCellState.Selected)
                {
                    cell.GoDisable();
                }                
            }
            yield return new WaitForSeconds(1);
            cells = GetNextCells(CurrentAnwser[i - 1]);
            foreach (CharacterCell cell in cells)
            {
                if (cell.State == CharacterCellState.Disable)
                {
                    cell.GoEnable();
                }
            }
            yield return new WaitForSeconds(1);
        }
        if (correctCro != null)
        {
            StartCoroutine(correctCro);
        }
    }

    private IEnumerator ShowResultCro()
    {
        ResetButton.interactable = false;
        for (int i = CurrentAnwser.Count; i < Anwser.Count; i++)
        {
            OnCellClicked(GetCharacterCell(Anwser[i]));     
            yield return new WaitForSeconds(2);
            StepText.text = "Success";
            StepText.transform.parent.GetComponent<Button>().interactable = false;           
        }
    }

    public void Reset()
    {
        int backCount = CurrentAnwser.Count - 1;
        StartCoroutine(ResetCro(backCount, null, false));
    }
}
