using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

// This Script represents the enemy shooting.
public class ShootingScript : MonoBehaviour
{
    [SerializeField]
    private GameObject BulletObject;

    [SerializeField]
    private AudioClip shootSoundEffect;

    [SerializeField]
    private List<GameObject> enemyTargets;

    private int currentTargetIndex = -1;

    [SerializeField]
    private AudioClip expSoundEffect;

    [SerializeField]
    private GameObject explosion;
    private float explosionDestroyDelay = 0.75f;

    private float nextLvlDely = 0.75f;

    private Boolean hasTarget = false;

    //spawn bullet objects and let them move towards a target point.
    protected virtual GameObject SpawnBullet()
    {
        GameObject shootTarget = enemyTargets[currentTargetIndex];

        Vector3 positionOfSpawnedObject = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );
        Quaternion rotationOfSpawnedObject = Quaternion.identity;
        GameObject newObject = Instantiate(
            BulletObject,
            positionOfSpawnedObject,
            rotationOfSpawnedObject
        );

        Mover newObjectMover = newObject.GetComponent<Mover>();
        if (newObjectMover)
        {
            newObjectMover.setTargetPoint(shootTarget.transform.position);
        }
        AudioSource.PlayClipAtPoint(shootSoundEffect, transform.position);

        return newObject;
    }

    void Update()
    {
        string pressedKey = Input.inputString; //get the pressed key
        if (!string.IsNullOrEmpty(pressedKey) && enemyTargets.Count > 0)
        {
            CheckTarget(pressedKey);

            TMP_Text currentText = enemyTargets[
                currentTargetIndex
            ].GetComponentInChildren<TMP_Text>();
            char currentChar = currentText.text[0];
            if (pressedKey[0] == currentChar)
            {
                SpawnBullet();
                currentText.text = currentText.text.Remove(0, 1);
            }

            if (currentText.text.Length == 0)
            {
                CompleteWord();
            }
        }
    }

    //check if we have a target or not, if we dont find new one, if we already typing a target dont change.
    void CheckTarget(string pressedKey)
    {
        if (!hasTarget)
        {
            currentTargetIndex = FindTarget(pressedKey[0]);
            GameObject shootTarget = enemyTargets[currentTargetIndex];
            Mover bulletMover = BulletObject.GetComponent<Mover>();
            if (bulletMover)
            {
                bulletMover.setTargetPoint(shootTarget.transform.position);
            }

            if (currentTargetIndex < 0)
            {
                return;
            }

            hasTarget = true;
        }
    }

    //find the next target that starts with char that we first typed
    int FindTarget(char searchChar)
    {
        for (int i = 0; i < enemyTargets.Count; i++)
        {
            string enemyText = enemyTargets[i].GetComponentInChildren<TMP_Text>().text;

            if (enemyText[0] == searchChar)
            {
                Debug.Log(enemyText);
                return i;
            }
        }
        return -1;
    }

    //destroy the word and do sound and expl effects.
    void CompleteWord()
    {
        AudioSource.PlayClipAtPoint(expSoundEffect, transform.position);
        GameObject exp = Instantiate(
            explosion,
            enemyTargets[currentTargetIndex].transform.position,
            enemyTargets[currentTargetIndex].transform.rotation
        );
        Destroy(enemyTargets[currentTargetIndex]);
        Destroy(exp, explosionDestroyDelay); //destroy the explosion object after a specifc time
        enemyTargets.RemoveAt(currentTargetIndex);
        hasTarget = false;

        if (enemyTargets.Count <= 0)
        {
            Debug.Log("fisnished");
            Invoke("GoToNextLvl", nextLvlDely);
        }
    }

    //move to the next lvl
    public void GoToNextLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
