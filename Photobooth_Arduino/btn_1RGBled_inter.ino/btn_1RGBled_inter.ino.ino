#include <U8g2lib.h>
// Pins pour l'OLED (soudé sur le module)
#define OLED_SDA 5
#define OLED_SCL 6
// OLED 72x40 sur son propre bus I2C (Software I2C)
U8G2_SSD1306_72X40_ER_F_SW_I2C u8g2(U8G2_R0, OLED_SCL, OLED_SDA, U8X8_PIN_NONE);

const int redPin = 7;
const int greenPin = 8;
const int bluePin = 9;
const int btnPin = 10;

int intensity = 10;

volatile bool next = false;
volatile unsigned long lastInterruptTime = 0;
const unsigned long DEBOUNCE_DELAY_ms = 200;


//interruption pour ESP32
void IRAM_ATTR onEvent() {
  unsigned long now = millis();
  if (now - lastInterruptTime > DEBOUNCE_DELAY_ms) {
    next = true;
    lastInterruptTime = now;
  }
}

void setup_OLED() {
  // Initialiser l'OLED
  u8g2.begin();
  u8g2.setContrast(255);

  u8g2.clearBuffer();
  u8g2.setFont(u8g2_font_5x7_tr);
  u8g2.drawStr(2, 8, "INIT");
  u8g2.drawStr(2, 18, "OLED OK");
  u8g2.sendBuffer();
}


void setup() {
  setup_OLED();
  pinMode(redPin, OUTPUT);
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);

  pinMode(btnPin, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(btnPin), onEvent, RISING);  // ← RISING car NC
  Serial.begin(9600);
}

void WaitNext() {
  Serial.println("waiting");
  while (!next)
    yield();  // yield() au lieu de delay() sur ESP32
  next = false;
}

void loop() {
  Serial.println("R");
  setColor(intensity, 0, 0);
  WaitNext();
  LedOFF();
  WaitNext();

  Serial.println("G");
  setColor(0, intensity, 0);
  WaitNext();
  LedOFF();
  WaitNext();

  Serial.println("B");
  setColor(0, 0, intensity);
  WaitNext();
  LedOFF();
  WaitNext();
}

void OLED_print(String val_txt, int x, int y) {
  x = 10;
  y = 15;
  u8g2.clearBuffer();
  u8g2.setFont(u8g2_font_9x15_tr);
  u8g2.drawStr(x, y, val_txt.c_str());
  u8g2.sendBuffer();
}

void LedOFF() {
  setColor(0, 0, 0);
}

void setColor(int r, int g, int b) {
  analogWrite(redPin, r);
  analogWrite(greenPin, g);
  analogWrite(bluePin, b);
}