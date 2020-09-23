using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class controller_player : MonoBehaviour
{

    public Rigidbody jogador;
    public GameObject cenario;
    public float velocidadeCenario;
    
    public GameObject bloco;
    public GameObject moeda;
    public GameObject diamond;
    public GameObject mola;

    private int estagioAtual = 1;

    public float distanciaRaia;
    private int raiaAtual;
    private Vector3 target;
    public Text gameoverText;

    private Vector2 initialPosition;

    private AudioSource somMoeda;
    private AudioSource music;
    private AudioSource somDiamond;
    private AudioSource somExplosao;

    private bool isGameOver = false;

    public Text txtPontos;
    private int pontos = 0;

    private bool pulando = false;
    private bool caindo = false;
    private int countCenario = 0;
    private bool diamondCreate = false;
    private bool diamondGetting = false;

    // Start is called before the first frame update
    void Start()
    {

        somMoeda = GetComponents<AudioSource>()[0];
        somExplosao = GetComponents<AudioSource>()[1];
        music = GetComponents<AudioSource>()[2];
        somDiamond = GetComponents<AudioSource>()[3];

        music.Play();
        raiaAtual = 1;
        target = jogador.transform.position;
        montarCenario();
        txtPontos.text = "" + pontos;
    }

    // Update is called once per frame
    void Update()
    {
        
        

        if (isGameOver)
        {
            gameoverText.text = "Game Over";
            return;
        }

        if( diamondGetting )
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("obstaculo");
            foreach (GameObject enemy in enemies)
                GameObject.Destroy(enemy);
        }


        int novaRaia = -1;

        bool pular = false;

        if (Input.GetKeyDown(KeyCode.RightArrow) && raiaAtual < 2)
        {
            novaRaia = raiaAtual + 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && raiaAtual > 0)
        {
            novaRaia = raiaAtual - 1;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pular = true;
        }


        //mouse
        if (Input.GetMouseButtonDown(0))
        {
            initialPosition = Input.mousePosition;
        }else if (Input.GetMouseButtonUp(0))
        {
            if(Input.mousePosition.x > initialPosition.x && raiaAtual < 2)
            {
                novaRaia = raiaAtual + 1;
            }else if(Input.mousePosition.x < initialPosition.x && raiaAtual > 0)
            {
                novaRaia = raiaAtual - 1;
            }

            if(Input.mousePosition.y > initialPosition.y)
            {
                pular = true;
            }
        }

        //touch
        if( Input.touchCount >= 1)
        {
            if( Input.GetTouch(0).phase == TouchPhase.Began)
            {
                initialPosition = Input.GetTouch(0).position;
            }else if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            {
                if( Input.GetTouch(0).position.x > initialPosition.x && raiaAtual < 2)
                {
                    novaRaia = raiaAtual + 1;
                }else if(Input.GetTouch(0).position.x > initialPosition.x && raiaAtual > 0)
                {
                    novaRaia = raiaAtual - 1;
                }
            }

            if(Input.GetTouch(0).position.y > initialPosition.y)
            {
                pular = true;
            }
        }
        if (novaRaia >= 0)
        {
            raiaAtual = novaRaia;
            target = new Vector3((raiaAtual - 1) * distanciaRaia, jogador.transform.position.y, jogador.transform.position.z);
            //jogador.transform.position = pos;
        }
        
        if (pular)
        {
            pulando = true;
        }

        if (pulando)
        {
            if (jogador.transform.position.y < 2.5 && caindo == false)
            {
                target.y = 3.0F;
                caindo = false;
                jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 5 * Time.deltaTime);
            }
            else
            {
                pulando = false;
                caindo = false;
            }
        } else if(pulando == false && jogador.transform.position.y > 0.5 )
        {
            target.y = 0.5F;
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 3 * Time.deltaTime);
               
        } else if (jogador.transform.position.x != target.x)
        {
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 2 * Time.deltaTime);
        }
        

        velocidadeCenario += (Time.deltaTime * 0.1F);
        cenario.transform.Translate(0, 0, velocidadeCenario * Time.deltaTime * -1);

        float cenarioz = cenario.transform.position.z;
        float estagio = Mathf.Floor((cenarioz - 80.0F)/-100.0F) + 1;
        if(estagio > estagioAtual)
        {
           
            estagioAtual++;
            countCenario++;
            montarCenario();
        }

    }

    void montarCenario()
    {
        for(int i= 2; i < 10; i++)
        {
            int[] elemento = new int[3];
            for(int j=0; j < 3; j++)
            {
                elemento[j] = Random.Range(0, 4);
                //0 = nada; 1 = bloco; 2 = moeda;

                if(countCenario == 1 && !diamondCreate) 
                {
                    Debug.Log("diamante");
                    instaciarDiamond(i, j);
                    diamondCreate = true;
                    i++;
                }

                if (elemento[0] == 1 && elemento[1] == 1 && elemento[2] == 1)
                {
                    elemento[2] = 0;
                }
                if(elemento[j] == 1 && !diamondGetting)
                {
                    instaciarBloco(i, j);
                }else if(elemento[j] == 2)
                {
                    instaciarMoeda(i, j);
                }else if(elemento[j] == 3)
                {
                    instaciarMola(i, j);
                }
            }
        }
    }

    void instaciarBloco(int indicez, int indicex)
    {
        GameObject bloco2 = Instantiate(bloco);
        float posz = ((10* indicez) + (estagioAtual-1) * 100 + cenario.transform.position.z);
        float posx = (indicex - 1) * distanciaRaia;
        bloco2.transform.SetParent(cenario.transform);
        bloco2.transform.position = new Vector3(posx, 0.5F, posz);
    }

    void instaciarDiamond(int indicez, int indicex)
    {
        GameObject diamond2 = Instantiate(diamond);
        float posz = ((10 * indicez) + (estagioAtual - 1) * 100 + cenario.transform.position.z);
        float posx = (indicex - 1) * distanciaRaia;
        diamond2.transform.SetParent(cenario.transform);
        diamond2.transform.position = new Vector3(posx, 0.5F, posz);
    }

    void instaciarMoeda(int indicez, int indicex)
    {
        GameObject moeda2 = Instantiate(moeda);
        float posz = ((10 * indicez) + (estagioAtual - 1) * 100 + cenario.transform.position.z);
        float posx = (indicex - 1) * distanciaRaia;
        moeda2.transform.SetParent(cenario.transform);
        moeda2.transform.position = new Vector3(posx, 0.5F, posz);
    }

    void instaciarMola(int indicez, int indicex)
    {
        GameObject mola2 = Instantiate(mola);
        float posz = ((10 * indicez) + (estagioAtual - 1) * 100 + cenario.transform.position.z);
        float posx = (indicex - 1) * distanciaRaia;
        mola2.transform.SetParent(cenario.transform);
        mola2.transform.position = new Vector3(posx, 0.5F, posz);
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("gold"))
        {
            somMoeda.Play();
            Destroy(other.gameObject);
            pontos++;
            txtPontos.text = "" + pontos;
           
        }
        if (other.gameObject.CompareTag("obstaculo"))
        {
            somExplosao.Play();
            isGameOver = true;
            Invoke("GameOver", 3);
            
        }

        if (other.gameObject.CompareTag("diamond"))
        {
            music.Stop();
            somDiamond.Play();
            Destroy(other.gameObject);
            diamondGetting = true;
            
        }
        if (other.gameObject.CompareTag("mola"))
        {
            target.y = 3.0F;
            caindo = false;
            jogador.transform.position = Vector3.Lerp(jogador.transform.position, target, 50 * Time.deltaTime);
        }
    }

    void GameOver()
    {
         SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }


}

