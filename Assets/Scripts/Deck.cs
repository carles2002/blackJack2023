using UnityEngine;
using UnityEngine.UI;
using System;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

   

    public int[] values = new int[52];
    int cardIndex = 0;

    //Primer movimiento
    private bool isFirstMove = true;

    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int[] tempValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13
            , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13
            , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13
            , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        Array.Copy(tempValues, values, 52);




        //Mostar valores por console
        /*
        for (int i = 0; i<=52; i++)
        {
            Debug.Log("Card "+i+" : "+ values[i]);
        }
        */
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */


        for (int i = values.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = values[i];
            Sprite temp2 = faces[i];

            values[i] = values[randomIndex];
            faces[i] = faces[randomIndex];

            values[randomIndex] = temp;
            faces[randomIndex] = temp2;
        }

        //Mostar valores por console
        /*
        for (int i = 0; i <= 51; i++)
        {
            Debug.Log("Card " + i + " : " + values[i]);
        }
        */

    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            
            // Comprobar si el jugador o el repartidor tienen Blackjack
            if (player.GetComponent<CardHand>().points == 21 || dealer.GetComponent<CardHand>().points == 21)
            {
                hitButton.interactable = false;
                stickButton.interactable = false;

                if (player.GetComponent<CardHand>().points == 21 && dealer.GetComponent<CardHand>().points == 21)
                {
                    finalMessage.text = "Empate: Ambos tienen Blackjack!";
                }
                else if (player.GetComponent<CardHand>().points == 21)
                {
                    finalMessage.text = "Ganaste: ¡Tienes Blackjack!";
                }
                else
                {
                    finalMessage.text = "Perdiste: El dealer tiene Blackjack";
                }
            }


        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        // Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
        if (isFirstMove)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
            isFirstMove = false;
            
        }

        // Repartimos carta al jugador
        PushPlayer();

        // Comprobamos si el jugador ya ha perdido y mostramos mensaje
        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Perdiste, te has pasado de 21.";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }


    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
        * Repartimos cartas al dealer si tiene 16 puntos o menos
        * El dealer se planta al obtener 17 puntos o más
        * Mostramos el mensaje del que ha ganado
        */

        // Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
        if (isFirstMove)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
            isFirstMove = false;
        }

        // Repartimos cartas al dealer si tiene 16 puntos o menos
        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }

        // El dealer se planta al obtener 17 puntos o más
        int dealerPoints = dealer.GetComponent<CardHand>().points;
        int playerPoints = player.GetComponent<CardHand>().points;

        // Mostramos el mensaje del que ha ganado
        if (dealerPoints > 21 || playerPoints > dealerPoints)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
            finalMessage.text = "Has ganado!";
        }
        else if (playerPoints == dealerPoints)
        {
            finalMessage.text = "Empate!";
        }
        else
        {
            dealer.GetComponent<CardHand>().InitialToggle();
            finalMessage.text = "El dealer gana!";
        }

        // Desactivamos los botones de Hit y Stand
        hitButton.interactable = false;
        stickButton.interactable = false;

       

    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}
