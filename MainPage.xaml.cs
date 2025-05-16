using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Data.Xml.Dom;
using System.Diagnostics;
using System.Globalization;
using HtmlAgilityPack;
using System.Xml;

namespace waluty
{
    public class PozycjaTabeliA
    {
        public string NazwaWaluty { get; set; }
        public string KodWaluty { get; set; }
        public string KursSredni { get; set; }
    }

    public sealed partial class MainPage : Page
    {
        private List<PozycjaTabeliA> kursyAktualne = new List<PozycjaTabeliA>();

        public MainPage()
        {
            InitializeComponent();

        }

        private void txtKwota_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Twój kod obsługi zmian w polu tekstowym txtKwota
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ContentPanel_Loaded(null, null);
        }

        // Metoda do ładowania danych o kursach walut
        private void ContentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Metoda ContentPanel_Loaded została wywołana.");

                string filePath = "Assets/kursy.xml"; // Ścieżka do Twojego pliku XML
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(filePath);

                System.Xml.XmlNodeList pozycje = doc.SelectNodes("//pozycja");

                if (pozycje != null)
                {
                    kursyAktualne.Clear();

                    foreach (XmlNode pozycja in pozycje)
                    {
                        string nazwaWaluty = pozycja.SelectSingleNode("nazwa_waluty").InnerText.Trim();
                        string kodWaluty = pozycja.SelectSingleNode("kod_waluty").InnerText.Trim();
                        string kursSredni = pozycja.SelectSingleNode("kurs_sredni").InnerText.Trim();

                        PozycjaTabeliA pozycjaTabeliA = new PozycjaTabeliA
                        {
                            NazwaWaluty = nazwaWaluty,
                            KodWaluty = kodWaluty,
                            KursSredni = kursSredni
                        };

                        kursyAktualne.Add(pozycjaTabeliA);
                    }

                    cbxZWaluty.ItemsSource = kursyAktualne;
                    cbxNaWalute.ItemsSource = kursyAktualne;

                    Debug.WriteLine("Dane o kursach walut zostały pomyślnie załadowane i ustawione jako źródła danych dla ComboBox.");
                }
                else
                {
                    Debug.WriteLine("Brak danych o kursach walut.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Wystąpił błąd podczas ładowania danych: " + ex.Message);
            }
        }

        private void Przelicz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pobieramy wybrane waluty i wprowadzoną kwotę
                PozycjaTabeliA zWaluty = (PozycjaTabeliA)cbxZWaluty.SelectedItem;
                PozycjaTabeliA naWalute = (PozycjaTabeliA)cbxNaWalute.SelectedItem;

                // Sprawdzamy, czy wybrane są obie waluty
                if (zWaluty == null || naWalute == null)
                {
                    Debug.WriteLine("Wybierz obie waluty do konwersji.");
                    return;
                }

                // Sprawdzamy, czy wprowadzono kwotę
                if (string.IsNullOrWhiteSpace(txtKwota.Text))
                {
                    Debug.WriteLine("Wprowadź kwotę do konwersji.");
                    return;
                }

                double kwota;
                // Próbujemy przekonwertować wprowadzoną kwotę na liczbę
                if (!double.TryParse(txtKwota.Text, out kwota))
                {
                    Debug.WriteLine("Niepoprawny format wprowadzonej kwoty.");
                    return;
                }

                // Dodajemy komunikaty debugowania do śledzenia wartości zmiennych
                Debug.WriteLine("KursSredni dla zWaluty: " + zWaluty.KursSredni);
                Debug.WriteLine("KursSredni dla naWalute: " + naWalute.KursSredni);

                // Wykonujemy konwersję
                double kursZ = Convert.ToDouble(zWaluty.KursSredni, CultureInfo.InvariantCulture);
                double kursNa = Convert.ToDouble(naWalute.KursSredni, CultureInfo.InvariantCulture);
                double wynik = (kwota * kursZ) / kursNa;

                // Aktualizujemy tekstowy blok z wynikiem
                txtWynik.Text = $"Wynik konwersji: {wynik:F2}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Wystąpił błąd podczas konwersji: " + ex.Message);
            }
        }

        private void cbxZWaluty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxZWaluty.SelectedItem != null)
            {
                PozycjaTabeliA selectedCurrency = (PozycjaTabeliA)cbxZWaluty.SelectedItem;
                Debug.WriteLine("Wybrana waluta Z: " + selectedCurrency.NazwaWaluty);
            }
        }

        private void cbxNaWalute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxNaWalute.SelectedItem != null)
            {
                PozycjaTabeliA selectedCurrency = (PozycjaTabeliA)cbxNaWalute.SelectedItem;
                Debug.WriteLine("Wybrana waluta na: " + selectedCurrency.NazwaWaluty);
            }
        }


        private void btnOProgramie_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(OProgramie));
        }

        private void btnPomoc_Click(object sender, RoutedEventArgs e)
        {
            // Przekazanie aktualnie wybranej waluty jako parametru do strony Pomoc
            this.Frame.Navigate(typeof(Pomoc), "Aktualnie wybrana waluta: EUR");
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void txtWynik_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
