//Prendre ESP32C3 Dev Module
//USB CDC On Boot: Enabled


// Ecran OLED intégré au ESP32-C3
#include <U8g2lib.h>
// Pins pour l'OLED (soudé sur le module)
#define OLED_SDA 5
#define OLED_SCL 6
// OLED 72x40 sur son propre bus I2C (Software I2C)
U8G2_SSD1306_72X40_ER_F_SW_I2C u8g2(U8G2_R0, OLED_SCL, OLED_SDA, U8X8_PIN_NONE);

//Bandes RGB : strip + ring
bool strip_allowed = true;
bool ring_allowed = true;

#include <Adafruit_NeoPixel.h>
#define LED_STRIP_PIN 7  //strip
#define LED_RING_PIN 8   //ring
const int LED_STRIP_COUNT = 120;
const int LED_RING_COUNT = 24;
const int STRIP_BRIGHTNESS = 10;//50;  //50 // Set BRIGHTNESS to about 1/5 (max = 255)
const int RING_BRIGHTNESS = 40;   //50 // Set BRIGHTNESS to about 1/5 (max = 255)
Adafruit_NeoPixel strip(LED_STRIP_COUNT, LED_STRIP_PIN, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel ring(LED_RING_COUNT, LED_RING_PIN, NEO_GRB + NEO_KHZ800);
int sLEDS[120];
int sleds[120];
int rleds[24];
uint32_t color = strip.Color(255, 0, 255);
uint32_t color_off = strip.Color(0, 0, 0);

//Appareil photo
#define APN_PIN 4

//Bouton
#define BTN_PIN 3

//variables internes
volatile bool bouton = false;
volatile unsigned long lastInterruptTime = 0;
const unsigned long DEBOUNCE_DELAY_ms = 200;


int temps_avant_mise_en_veille_min = 2;
unsigned long temps_avant_mise_en_veille_ms;
unsigned long temps_last_photo;

// bouton (interruption pour ESP32)
void IRAM_ATTR onEvent() {
  unsigned long now = millis();
  if (now - lastInterruptTime > DEBOUNCE_DELAY_ms) {
    bouton = true;
    lastInterruptTime = now;
  }
}

void setup_OLED() {
  u8g2.begin();
  u8g2.setContrast(255);

  u8g2.clearBuffer();
  u8g2.setFont(u8g2_font_5x7_tr);
  u8g2.drawStr(2, 8, "INIT");
  u8g2.drawStr(2, 18, "OLED OK");
  u8g2.sendBuffer();
}

void setup_strip() {
  strip.begin();  // INITIALIZE NeoPixel strip object
  strip.show();   // Turn OFF all pixels ASAP
  strip.setBrightness(STRIP_BRIGHTNESS);

  //init strip's leds
  int indexmax = strip.numPixels();
  for (int i = 0; i < (int)(indexmax / 2); i++)  // For each pixel in strip...
  {
    sleds[i] = i * 2;
    sleds[indexmax - i - 1] = i * 2 + 1;
  }

  for (int i = 0; i < indexmax; i++)    // For each pixel in strip
    for (int j = 0; j < indexmax; j++)  // For each pixel in strip
      if (sleds[j] == i)
        sLEDS[i] = j;
}

void setup_ring() {
  ring.begin();  // INITIALIZE NeoPixel ring object
  ring.show();   // Turn OFF all pixels ASAP
  ring.setBrightness(RING_BRIGHTNESS);

  //init ring's leds
  int indexmax = ring.numPixels();
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
  attachInterrupt(digitalPinToInterrupt(BTN_PIN), onEvent, RISING);  // RISING car sur NC
}

void setup_appareil_photo() {
  pinMode(APN_PIN, OUTPUT);
}

void setup() {
  Serial.begin(9600);

  setup_OLED();
  OLED_clear();
  OLED_print("oled ok", 2, 8);
  if (strip_allowed) {
    setup_strip();
    OLED_print("strip ok", 2, 18);
  }
  if (ring_allowed) {
    setup_ring();
    OLED_print("ring ok", 2, 28);
  }
  setup_bouton();
  setup_appareil_photo();
  OLED_print("ready", 2, 38);
  temps_avant_mise_en_veille_ms = temps_avant_mise_en_veille_min * 60000;
  temps_last_photo = 0;
}

void loop() {
  if (temps_avant_mise_en_veille_ms < 0)
    photo();

  // effet arc en ciel sur les bandes de leds
  rainbowFade2White(1, 1, 0);

  // // test si réglage delais à changer
  // int val = analogRead(potPin);  //ajout de 1,5 seconde de temps sur le ring

  // int val_sec = val / 100;
  // int val_csec = val - val_sec * 100;
  // val_delay_cs = val_sec + val_csec / 100;

  //tftOnScreen(String(val_sec), String(val_csec));

  //temps avant prise de photo auto
  temps_avant_mise_en_veille_ms = millis() - temps_last_photo;
}

void tftOnScreen(String CHAINE_sec, String CHAINE_csec) {
  // //temps entre bouton et photo
  // String CHAINE;
  // if (val_delay_cs_prec != val_delay_cs)
  // {
  //   CHAINE = CHAINE_sec +  "," + CHAINE_csec;
  //   tft.fillRect(30, 0, largeur_digit * 7, 7, RED);
  //   tft.setTextColor(GREEN);
  //   if(CHAINE.length()==1)
  //     tft.setCursor(largeur_digit * (7 + 4),0);
  //   if(CHAINE.length()==2)
  //     tft.setCursor(largeur_digit * (7 + 3),0);
  //   if(CHAINE.length()==3)
  //     tft.setCursor(largeur_digit * (7 + 2),0);
  //   if(CHAINE.length()==4)
  //     tft.setCursor(largeur_digit * (7 + 1),0);
  //   if(CHAINE.length()==5)
  //     tft.setCursor(largeur_digit * (7 + 0),0);

  //   tft.print(CHAINE);
  //   Serial.println(CHAINE);
  //   val_delay_cs_prec = val_delay_cs;
  // }


  // CHAINE = String(temps_avant_mise_en_veille_ms);
  // tft.setCursor(11,10);
  // tft.print("auto (ms)");
  // tft.print(CHAINE);
}

void OLED_print(String val_txt, int x, int y) {
  u8g2.setFont(u8g2_font_5x7_tr);  //u8g2_font_5x7_tr  u8g2_font_9x15_tr
  u8g2.drawStr(x, y, val_txt.c_str());
  u8g2.sendBuffer();
}

void OLED_clear() {
  u8g2.clearBuffer();
}



void photo() {

  Serial.print("Photo - délais en ms : ");
  long now = millis();
  //timer sur leds
  AllLedsOn();
  TempoLedsStrip(5);  //val_delay_cs);
  Serial.print(millis() - now);
  Serial.print(" -> ");
  TempoLedsRing();

  //enclenchement prise de photo
  digitalWrite(APN_PIN, LOW);
  Serial.println(millis() - now);

  //dés-enclenchement prise de photo
  digitalWrite(APN_PIN, HIGH);

  //tempo pour empêcher la mise en veille de l'appareil photo
  temps_last_photo = now + temps_avant_mise_en_veille_ms;
}

void AllLedsOn() {
  //tout ON
  for (int i = 0; i < strip.numPixels(); i++) {  // For each pixel in strip...
    strip.setPixelColor(sLEDS[i], color);        //  Set pixel's color
  }

  for (int i = 0; i < ring.numPixels(); i++) {  // For each pixel in strip...
    ring.setPixelColor(rleds[i], color);        //  Set pixel's color
  }

  strip.show();  //  Update strip to match
  ring.show();
}

void TempoLedsStrip(float temps_s) {
  if (temps_s < 0)
    temps_s = 0;

  //EXTINCTION TEMPORISE LES UNES APRES LES AUTRES
  int tps = (int)(1000 * temps_s / strip.numPixels());
  for (int i = 0; i < strip.numPixels(); i++) {  // For each pixel in strip...
    strip.setPixelColor(sLEDS[i], color_off);    //  Set pixel's color (in RAM)
    strip.show();                                //  Update strip to match
    delay(tps);
  }
}

void TempoLedsRing() {
  for (int j = 0; j < 6; j++) {
    for (int i = 0; i < ring.numPixels(); i++)
      ring.setPixelColor(rleds[i], color);
    ring.show();
    delay(125);

    for (int i = 0; i < ring.numPixels(); i++)
      ring.setPixelColor(rleds[i], color_off);
    ring.show();
    delay(125);
  }
}


void rainbowFade2White(int wait, int rainbowLoops, int whiteLoops) {
  int fadeVal = 100;
  int fadeMax = 100;

  // Hue of first pixel runs 'rainbowLoops' complete loops through the color
  // wheel. Color wheel has a range of 65536 but it's OK if we roll over, so
  // just count from 0 to rainbowLoops*65536, using steps of 256 so we
  // advance around the wheel at a decent clip.

  for (uint32_t firstPixelHue = 0; firstPixelHue < (uint32_t)rainbowLoops * 65536UL; firstPixelHue += 256) {
    if (strip_allowed) {
      for (int i = 0; i < strip.numPixels(); i++) {  // For each pixel in strip...
        // Offset pixel hue by an amount to make one full revolution of the
        // color wheel (range of 65536) along the length of the strip
        // (strip.numPixels() steps):
        uint32_t pixelHue = firstPixelHue + (i * 65536L / strip.numPixels());

        // strip.ColorHSV() can take 1 or 3 arguments: a hue (0 to 65535) or
        // optionally add saturation and value (brightness) (each 0 to 255).
        // Here we're using just the three-argument variant, though the
        // second value (saturation) is a constant 255.
        strip.setPixelColor(i, strip.gamma32(strip.ColorHSV(pixelHue,
                                                            255,
                                                            255 * fadeVal / fadeMax)));
      }
    }

    if (ring_allowed) {
      for (int i = 0; i < ring.numPixels(); i++) {
        uint32_t pixelHue = firstPixelHue + (i * 65536L / ring.numPixels());
        ring.setPixelColor(i, ring.gamma32(ring.ColorHSV(pixelHue,
                                                         255,
                                                         255 * fadeVal / fadeMax)));
      }
    }

    if (strip_allowed)
      strip.show();

    if (ring_allowed)
      ring.show();

    delay(wait);
  }
}