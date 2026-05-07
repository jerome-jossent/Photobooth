#pragma region TFT
/*
 * Écran TFT 1,44" - 128x128 pixels (ST7735)
 * Compatible Arduino Uno
 * BIBLIOTHÈQUES REQUISES :
 * - Adafruit ST7735 and ST7789 Library
 * (incluse - Adafruit GFX Library)
 *
 * BRANCHEMENT (module SPI) :
 * ---------------------------
 * TFT VCC   -->  5V  (ou 3.3V selon votre module)
 * TFT GND   -->  GND
 * TFT SCL   -->  Pin 13 (SCK)
 * TFT SDA   -->  Pin 11 (MOSI)
 * TFT RES   -->  Pin 8
 * TFT DC    -->  Pin 9
 * TFT CS    -->  Pin 10
 * TFT BL    -->  5V  (rétroéclairage toujours allumé)
 */
#include <Adafruit_GFX.h>
#include <Adafruit_ST7735.h>
#include <SPI.h>

#define TFT_CS 10
#define TFT_RST 9
#define TFT_DC 8

Adafruit_ST7735 tft = Adafruit_ST7735(TFT_CS, TFT_DC, TFT_RST);

#define NOIR ST77XX_BLACK
#define BLANC ST77XX_WHITE
#define ROUGE ST77XX_RED
#define VERT ST77XX_GREEN
#define BLEU ST77XX_BLUE
#define JAUNE ST77XX_YELLOW
#define CYAN ST77XX_CYAN
#define MAGENTA ST77XX_MAGENTA
#define ORANGE 0xFD20
#define VIOLET 0x8010
#define GRIS 0x7BEF

uint16_t COULEUR_FOND;
#pragma endregion

#pragma region Buffer série
String inputString = "";
bool commandeComplete = false;
#pragma endregion

#pragma region Bandes RGB : strip + ring
bool strip_allowed = true;
bool ring_allowed = true;

#include <Adafruit_NeoPixel.h>
#define LED_STRIP_PIN 3
#define LED_RING_PIN 4
const int LED_STRIP_COUNT = 120;
const int LED_RING_COUNT = 24;
const int STRIP_BRIGHTNESS = 50;
const int RING_BRIGHTNESS = 40;
Adafruit_NeoPixel strip(LED_STRIP_COUNT, LED_STRIP_PIN, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel ring(LED_RING_COUNT, LED_RING_PIN, NEO_GRB + NEO_KHZ800);
int sLEDS[120];
int sleds[120];
int rleds[24];
uint32_t color = strip.Color(255, 0, 255);
uint32_t color_off = strip.Color(0, 0, 0);
#pragma endregion

#pragma region Appareil photo
#define APN_PIN 7
bool take_photo;
#pragma endregion

#pragma region Bouton
#define BTN_PIN 2
#define BTN_LED_PIN 6
volatile bool btn_pushed = false;
#pragma endregion

#pragma region variables internes
volatile bool interrompre_rainbow = false;

int tempoLedsRing_ms = 1000;
volatile unsigned long lastInterruptTime = 0;
const unsigned long DEBOUNCE_DELAY_ms = 1000;
int t_delay_photo_ms = 2000;
int temps_avant_mise_en_veille_min = 2;
unsigned long temps_avant_mise_en_veille_ms;
unsigned long temps_last_photo;

// --- NOUVEAU : timer non-bloquant pour l'affichage couleur ---
unsigned long temps_affichage_couleur = 0;
#define DUREE_AFFICHAGE_COULEUR_MS 1000

int i = 0;
#pragma endregion

void bouton_interrupt() {
  if (digitalRead(BTN_PIN) == false)
    return;

  if (btn_pushed)
    return;

  unsigned long now = millis();
  if (now - lastInterruptTime > DEBOUNCE_DELAY_ms) {
    btn_pushed = true;
    Bouton_led(false);
    Serial.println("P");
    lastInterruptTime = now;
    interrompre_rainbow = true;
  }
}

#pragma region SETUPS

void setup_TFT() {
  tft.initR(INITR_GREENTAB);
  tft.setRotation(0);
  COULEUR_FOND = NOIR;
}

void setup_strip() {
  strip.begin();
  strip.show();
  strip.setBrightness(STRIP_BRIGHTNESS);

  int indexmax = strip.numPixels();
  for (int i = 0; i < (int)(indexmax / 2); i++) {
    sleds[i] = i * 2;
    sleds[indexmax - i - 1] = i * 2 + 1;
  }

  for (int i = 0; i < indexmax; i++)
    for (int j = 0; j < indexmax; j++)
      if (sleds[j] == i)
        sLEDS[i] = j;
}

void setup_ring() {
  ring.begin();
  ring.show();
  ring.setBrightness(RING_BRIGHTNESS);

  rleds[0] = 0;
  rleds[1] = 5;
  rleds[2] = 15;
  rleds[3] = 23;
  rleds[4] = 8;
  rleds[5] = 16;
  rleds[6] = 13;
  rleds[7] = 17;
  rleds[8] = 3;
  rleds[9] = 9;
  rleds[10] = 20;
  rleds[11] = 2;
  rleds[12] = 10;
  rleds[13] = 18;
  rleds[14] = 1;
  rleds[15] = 4;
  rleds[16] = 12;
  rleds[17] = 19;
  rleds[18] = 22;
  rleds[19] = 14;
  rleds[20] = 6;
  rleds[21] = 21;
  rleds[22] = 11;
  rleds[23] = 7;
}

void setup_bouton() {
  pinMode(BTN_PIN, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(BTN_PIN), bouton_interrupt, RISING);
  pinMode(BTN_LED_PIN, OUTPUT);
  Bouton_led(true);
}

void setup_appareil_photo() {
  pinMode(APN_PIN, OUTPUT);
  Take_photo(false);
}

void setup() {
  Serial.begin(9600);

  setup_TFT();
  TFT_clear();
  TFT_print("TFT ok", 2, 35);

  if (strip_allowed) {
    setup_strip();
    TFT_print("RGB strip ok", 2, 45);
  }
  if (ring_allowed) {
    setup_ring();
    TFT_print("RGB ring ok", 2, 55);
  }

  setup_bouton();
  TFT_print("BTN ok", 2, 65);

  delay(500);

  setup_appareil_photo();
  TFT_print("APN ok", 2, 75);

  temps_avant_mise_en_veille_ms = temps_avant_mise_en_veille_min * 60000;
  temps_last_photo = 0;

  TFT_print("Photobooth Ready", 2, 85);
}
#pragma endregion

void Println_TrueFalse(bool value) {
  if (value) Serial.println("True");
  else Serial.println("False");
}

void Take_photo(bool value) {
  digitalWrite(APN_PIN, !value);  // garder relais souvent au repos
  Serial.print("take_photo ");
  Println_TrueFalse(value);
}

void Bouton_led(bool value) {
  digitalWrite(BTN_LED_PIN, value);  // garder relais souvent au repos
  Serial.print("bouton_led ");
  Println_TrueFalse(value);
}

void TraiterCommande(String cmd) {

  TFT_print(cmd, 2, 120);

  if (cmd[0] == 'p') take_photo = true;

  if (cmd[0] == 'R' && cmd[1] == '1') Take_photo(true);
  if (cmd[0] == 'R' && cmd[1] == '2') Bouton_led(true);
  if (cmd[0] == 'r' && cmd[1] == '1') Take_photo(false);
  if (cmd[0] == 'r' && cmd[1] == '2') Bouton_led(false);

  // nouvelle couleur
  if (cmd[0] == 'c') {
    Serial.println("cmd brute reçue : [" + cmd + "]");
    cmd.remove(0, 2);  // enlève "c:"

    int r, g, b;
    int firstComma = cmd.indexOf(',');
    int secondComma = cmd.indexOf(',', firstComma + 1);

    if (firstComma > 0 && secondComma > firstComma) {
      r = cmd.substring(0, firstComma).toInt();
      g = cmd.substring(firstComma + 1, secondComma).toInt();
      b = cmd.substring(secondComma + 1).toInt();
      color = strip.Color(r, g, b);
      AllLedsOn();
      temps_affichage_couleur = millis();
      interrompre_rainbow = true;
    }
  }

  if (cmd[0] == 't') {
    t_delay_photo_ms = cmd.substring(1).toInt();

    if (!btn_pushed) {
      btn_pushed = true;
      Bouton_led(false);
      lastInterruptTime = millis();
    }

    Serial.print("t recu : ");
    Serial.println(t_delay_photo_ms);
    ProcessPhoto();
  }
}

// Méthode Arduino
void serialEvent() {
  LireSerial();
}

void LireSerial() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();
    if (inChar == '\n') {
      inputString.trim();
      if (inputString.length() > 0)
        TraiterCommande(inputString);
      inputString = "";
    } else if (inChar != '\r') {
      inputString += inChar;
    }
  }
}

void ProcessPhoto() {
  take_photo = false;
  i++;
  TFT_print(String(i), 20, 95);

  Serial.print("Photo - délais en ms : ");
  long now = millis();
  AllLedsOn();
  TempoLedsStrip(t_delay_photo_ms);
  Serial.print("PHOTO ");
  Serial.print(millis() - now);
  Serial.print(" -> ");
  TempoLedsRing();

  Take_photo(true);
  Serial.println(millis() - now);
  delay(1000);
  Take_photo(false);

  Bouton_led(true);
  btn_pushed = false;
  interrompre_rainbow = false;
  temps_last_photo = now + temps_avant_mise_en_veille_ms;
}


void loop() {
  if (temps_avant_mise_en_veille_ms < 0) {
    Serial.println("PHOTO AUTO pour annuler la mise en veille auto de l'APN");
    take_photo = true;
  }

  // fin du timer couleur fixe
  if (temps_affichage_couleur > 0 && millis() - temps_affichage_couleur > DUREE_AFFICHAGE_COULEUR_MS) {
      temps_affichage_couleur = 0;
      interrompre_rainbow = false;
  }

  // après ProcessPhoto()
  if (take_photo) {
      ProcessPhoto();
      //interrompre_rainbow = false;
  }

  // effet arc en ciel sur les bandes de leds
  RGBRainbowFade2White(1, 1, 0);

  temps_avant_mise_en_veille_ms = millis() - temps_last_photo;
}


#pragma region TFT
void TFT_print(String val_txt, int x, int y) {
  int taille_X = val_txt.length() * 6;
  tft.setTextSize(1);
  tft.fillRect(x, y, taille_X, 10, COULEUR_FOND);
  tft.setTextColor(CYAN);
  tft.setCursor(x, y);
  tft.println(val_txt);
  Serial.println("tft : " + val_txt);
}

void TFT_clear() {
  tft.fillScreen(COULEUR_FOND);
}
#pragma endregion


#pragma region RGB strip& ring
void AllLedsOn() {
  for (int i = 0; i < strip.numPixels(); i++)
    strip.setPixelColor(sLEDS[i], color);

  for (int i = 0; i < ring.numPixels(); i++)
    ring.setPixelColor(rleds[i], color);

  strip.show();
  ring.show();
}

void TempoLedsStrip(float temps_ms) {
  temps_ms -= tempoLedsRing_ms;

  if (temps_ms < 0)
    temps_ms = 0;

  int tps = (int)(temps_ms / strip.numPixels());
  for (int i = 0; i < strip.numPixels(); i++) {
    strip.setPixelColor(sLEDS[i], color_off);
    strip.show();
    delay(tps);
  }
}

void TempoLedsRing() {
  int nbrclignotement = 5;
  int tempo1LedsRing_ms = tempoLedsRing_ms / nbrclignotement;
  tempo1LedsRing_ms /= 2;

  for (int j = 0; j < nbrclignotement + 1; j++) {
    for (int i = 0; i < ring.numPixels(); i++)
      ring.setPixelColor(rleds[i], color);
    ring.show();
    delay(tempo1LedsRing_ms);

    for (int i = 0; i < ring.numPixels(); i++)
      ring.setPixelColor(rleds[i], color_off);
    ring.show();
    delay(tempo1LedsRing_ms);
  }
}

void RGBRainbowFade2White(int wait, int rainbowLoops, int whiteLoops) {
  int fadeVal = 100;
  int fadeMax = 100;

  for (uint32_t firstPixelHue = 0; firstPixelHue < (uint32_t)rainbowLoops * 65536UL; firstPixelHue += 256) {
    LireSerial();
    if (interrompre_rainbow) return;

    if (strip_allowed) {
      for (int i = 0; i < strip.numPixels(); i++) {
        uint32_t pixelHue = firstPixelHue + (i * 65536L / strip.numPixels());
        strip.setPixelColor(i, strip.gamma32(strip.ColorHSV(pixelHue, 255, 255 * fadeVal / fadeMax)));
      }
    }

    if (ring_allowed) {
      for (int i = 0; i < ring.numPixels(); i++) {
        uint32_t pixelHue = firstPixelHue + (i * 65536L / ring.numPixels());
        ring.setPixelColor(i, ring.gamma32(ring.ColorHSV(pixelHue, 255, 255 * fadeVal / fadeMax)));
      }
    }

    if (strip_allowed) strip.show();
    if (ring_allowed) ring.show();

    delay(wait);
  }
}
#pragma endregion
