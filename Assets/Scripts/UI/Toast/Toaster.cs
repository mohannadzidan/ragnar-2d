using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toaster : MonoBehaviour
{
    [SerializeField] private GameObject toastPrefab;  // Prefab that contains a Text component
    [SerializeField] private float toastDuration = 3f;  // How long the toast message lasts
    [SerializeField] private float toastHideDuration = 0.5f;  // How long the toast message lasts
    [SerializeField] private int maxDisplayedToasts = 3;  // Maximum number of toasts on screen at once
    [SerializeField] private float toastHeight = 24f;
    private List<GameObject> visibleToasts = new List<GameObject>();

    public void Toast(string message)
    {
        // Create a new toast from the prefab

        if (visibleToasts.Count >= maxDisplayedToasts)
        {
            visibleToasts.RemoveAt(0);
        }

        StartCoroutine(ToastShowRoutine(message));
    }


    private IEnumerator ToastShowRoutine(string message)
    {
        GameObject toast = Instantiate(toastPrefab, transform);
        visibleToasts.Add(toast);
        var toastText = toast.GetComponent<TMP_Text>();
        toastText.text = message;
        var color = new Color(toastText.color.r, toastText.color.g, toastText.color.b, 0);
        toast.transform.position = transform.position + Vector3.down * toastHeight;
        float velocity = toastHeight / toastHideDuration;
        float alphaVelocity = 1 / toastHideDuration;
        float elapsedTime = 0f;
        while (elapsedTime < toastHideDuration && visibleToasts.IndexOf(toast) > -1)
        {
            toast.transform.position += Vector3.up * (velocity * Time.deltaTime);
            color.a += alphaVelocity * Time.deltaTime;
            toastText.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(ToastRoutine(toast));
    }

    int GetOrder(GameObject toast)
    {
        return visibleToasts.Count - visibleToasts.IndexOf(toast) - 1;
    }
    private IEnumerator ToastRoutine(GameObject toast)
    {
        var elapsedTime = 0f;
        var isMoving = false;
        var startPosition = toast.transform.position;
        var toPosition = toast.transform.position;
        var t = 0f;
        while (elapsedTime < toastDuration)
        {
            var order = GetOrder(toast);
            if (order >= visibleToasts.Count)
            {
                StartCoroutine(ToastHideRoutine(toast));
                yield break;
            }
            var desiredPosition = transform.position + Vector3.up * (order * toastHeight);
            if (!isMoving && Vector3.SqrMagnitude(desiredPosition - toast.transform.position) > 0.001f)
            {
                startPosition = toast.transform.position;
                toPosition = desiredPosition;
                isMoving = true;
                t = 0f;
            }
            if (isMoving)
            {
                t = Math.Min(t + Time.deltaTime / toastHideDuration, 1);
                toast.transform.position = Vector3.Lerp(startPosition, toPosition, t);
                if (t == 1)
                {
                    startPosition = toPosition;
                    isMoving = false;
                }
                else if (toPosition != desiredPosition)
                {
                    isMoving = false;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        var index = visibleToasts.IndexOf(toast);
        if (index > 0) visibleToasts.RemoveAt(index);
        StartCoroutine(ToastHideRoutine(toast));
    }
    private IEnumerator ToastHideRoutine(GameObject toast)
    {
        float elapsedTime = 0f;
        float velocity = toastHeight / toastHideDuration;
        var toastText = toast.GetComponent<TMP_Text>();
        var color = new Color(toastText.color.r, toastText.color.g, toastText.color.b, 1);
        float alphaVelocity = 1 / toastHideDuration;
        while (elapsedTime < toastHideDuration)
        {
            toast.transform.position += Vector3.up * (velocity * Time.deltaTime);
            color.a -= alphaVelocity * Time.deltaTime;
            toastText.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(toast);
    }

    public static Toaster FindInScene()
    {
        return GameObject.Find("Toaster")?.GetComponent<Toaster>();
    }
}
