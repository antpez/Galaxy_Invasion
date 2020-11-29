﻿using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO.Ports;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{    
    [Header("Player")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float padding = 1f;
    [SerializeField] int health = 200;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.75f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;
    [SerializeField] GameObject laserSpawn;

    [Header("Projectile")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;

    private float xMin;
    private float xMax;

    private float yMin;
    private float yMax;
    private SpriteRenderer sprite;

    private Coroutine firingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        sprite = GetComponent<SpriteRenderer>();
    }
       
    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }
        
    private void Move()
    {
        // Store the value of changes to our x-axis
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        // New position of x-axis
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        
        // Store the value of changes to our Y-axis
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        // New position of Y-axis
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        // Change the position of the player 
        transform.position = new Vector2(newXPos, newYPos);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinously());            
        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinously()
    {
        while (true)
        {
           GameObject laser = Instantiate(
                laserPrefab,
                laserSpawn.transform.position,
                Quaternion.identity) as GameObject;

           laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
           AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);

            yield return new WaitForSeconds(projectileFiringPeriod);
        }       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
    }

    public void ChangeColor()
    {
        sprite.color = new Color
           (UnityEngine.Random.Range(0F, 1F),
            UnityEngine.Random.Range(0, 1F),
            UnityEngine.Random.Range(0, 1F));
    }
}
