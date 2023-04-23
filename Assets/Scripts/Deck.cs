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
    public Text PointsMessage;
    public Text DealerPointsMessage;



    public int[] values = new int[52];
    int cardIndex = 0;


    
    public int credit = 0;
    public Text creditShown;
    public int apuesta = 0;
    public Text apuestaShown;

    public Button diezc;
    public Button veintec;
    public Button treintac;
    public Button cuareintac;


    private bool hasBet = false;

    //Primer movimiento
    private bool isFirstMove = true;

    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        credit = 1000;
        creditShown.text = credit.ToString();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int[] tempValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10
            , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10
            , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10
            , 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };
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
        if (hasBet)
        {
            finalMessage.text = "";
            credit = credit - apuesta;
            creditShown.text = credit.ToString();

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
    }

    private void CalculateProbabilities()
    {
        // Calcular la probabilidad de que el crupier tenga una puntuación mayor que la del jugador
        float dealerHigherProb = CalculateDealerHigherProbability(player.GetComponent<CardHand>().points);

        // Calcular probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
        float playerDrawRangeProb = CalculatePlayerDrawRangeProbability(player.GetComponent<CardHand>().points, 17, 21);

        probMessage.text = "Dealer>Player: " + dealerHigherProb.ToString("P1") + "\n17<=X<=21: "+ playerDrawRangeProb.ToString("P2");

        // Calcular la probabilidad de que el jugador obtenga más de 21 si pide una carta
        //El rango de 100 a 22 ya que queremos saber si se pasa de 21.
        float playerBustProb = CalculatePlayerDrawRangeProbability(player.GetComponent<CardHand>().points, 22, 100);
        probMessage.text += "\nX>21: " + playerBustProb.ToString("P3");


    }

    // Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
    //prob=resultados favorables/resultados posibles
    float CalculateDealerHigherProbability(int playerScore)
    {
        int usefulCards = 0;
        int remainingCards = 52 - cardIndex;

        for (int i = 0; i < values.Length; i++)
        {
            int cardValue = values[i];

            if (cardValue != -1)
            {
                cardValue = Mathf.Min(cardValue, 10); // Las cartas con figura (J, Q, K) tienen un valor de 10

                if (cardValue + 10 > playerScore && cardValue != 1) // Si es un as, considera el valor de 11
                {
                    usefulCards++;
                }
                else if (cardValue == 1 && cardValue + 11 > playerScore)
                {
                    usefulCards++;
                }
            }
        }

        float probability = (float)usefulCards / remainingCards;
        return probability;
    }

    //Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
    float CalculatePlayerDrawRangeProbability(int playerScore, int minScore, int maxScore)
    {
        int usefulCards = 0; //cartas que podrían llevar al jugador a tener una puntuación entre 17 y 21 al pedir una carta.
        int remainingCards = 52 - cardIndex; //num de cartas restantes en el mazo 

        for (int i = 0; i < values.Length; i++) //verifica cada carta restante en el mazo.
        {
            //Si la carta en la posición actual de values no es -1(lo que indica que la carta ya ha sido usada),
            //se continúa con el siguiente paso. De lo contrario, se pasa a la siguiente iteración del ciclo.
            int cardValue = values[i]; 

            if (cardValue != -1)
            {
                cardValue = Mathf.Min(cardValue, 10); // Se obtiene el valor de la carta actual, las cartas con figura (J, Q, K) tienen un valor de 10
                int newScore = playerScore + cardValue;

                if (newScore >= minScore && newScore <= maxScore) //Si newScore está entre 17 y 21 se incrementa la variable usefulCards en 1.
                {
                    usefulCards++;
                }
            }
        }

        //prob=resultados favorables/resultados posibles
        float probability = (float)usefulCards / remainingCards;
        return probability;
    }

    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        CalculateProbabilities();

        //Muestra la puntuación de la carta destapada hasta que pulsamos stand
        if (dealer.GetComponent<CardHand>().cards.Count == 2)
        {
            int dealerFaceUpCardValue = values[cardIndex];
            DealerPointsMessage.text = values[cardIndex].ToString();
        }
    }

    void PushPlayer()
    {
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        PointsMessage.text = player.GetComponent<CardHand>().points.ToString();
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
 
       

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
            DealerPointsMessage.text = dealerPoints.ToString();
            finalMessage.text = "Has ganado!";
            credit = credit + apuesta*2;
            creditShown.text = credit.ToString();
        }
        else if (playerPoints == dealerPoints)
        {
            DealerPointsMessage.text = dealerPoints.ToString();
            finalMessage.text = "Empate!";
            credit = credit+apuesta;
            creditShown.text = credit.ToString();
        }
        else
        {  
            dealer.GetComponent<CardHand>().InitialToggle();
            DealerPointsMessage.text = dealerPoints.ToString();
            finalMessage.text = "El dealer gana!";
            
            creditShown.text = credit.ToString();
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

        diezc.interactable = true;
        veintec.interactable = true;
        treintac.interactable = true;
        cuareintac.interactable = true;

        apuesta = 0;
        hasBet = false;
        apuestaShown.text = apuesta.ToString();




        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        if (hasBet)
        {
            StartGame();
        }
    }

    public void suma10() {
        
        if (credit < 10)
        {
            finalMessage.text = "No puedes apostar";
        }
        else
        {
            apuesta += 10;
            apuestaShown.text = apuesta.ToString();
            diezc.interactable = false;
            veintec.interactable = false;
            treintac.interactable = false;
            cuareintac.interactable = false;
            hasBet = true;
            StartGame();
        }
        
        
    }
    public void suma20() {

        if (credit < 20)
        {
            finalMessage.text = "No puedes apostar";
        }
        else
        {
            apuesta += 20;
            apuestaShown.text = apuesta.ToString();
            diezc.interactable = false;
            veintec.interactable = false;
            treintac.interactable = false;
            cuareintac.interactable = false;
            hasBet = true;
            StartGame();
        }
        
    }
    public void suma30() {

        if (credit < 30)
        {
            finalMessage.text = "No puedes apostar";
        }
        else
        {
            apuesta += 30;
            apuestaShown.text = apuesta.ToString();
            diezc.interactable = false;
            veintec.interactable = false;
            treintac.interactable = false;
            cuareintac.interactable = false;
            hasBet = true;
            StartGame();
        }
        
    }
    public void suma40() {

        if (credit < 40)
        {
            finalMessage.text = "No puedes apostar";
        }
        else
        {
            apuesta += 40;
            apuestaShown.text = apuesta.ToString();
            diezc.interactable = false;
            veintec.interactable = false;
            treintac.interactable = false;
            cuareintac.interactable = false;
            hasBet = true;
            StartGame();
        }
        
    }

   
}
