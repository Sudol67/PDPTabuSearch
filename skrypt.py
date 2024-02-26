import pandas as pd
from pywinauto.application import Application
from pywinauto import Desktop
from time import sleep

# Ścieżka do pliku Excel i aplikacji
excel_path = 'C:\\Users\\Basia\\Desktop\\Testowanie.xlsx'
app_path = 'C:\\Users\\Basia\\Desktop\\Zaawansowane_programowanie.exe'

# Odczytanie danych z Excela
df = pd.read_excel(excel_path)

# Uruchomienie aplikacji
app = Application().start(app_path)
sleep(2)  # Czekaj na uruchomienie aplikacji

# Możesz użyć wydrukowanego tytułu, aby połączyć się bezpośrednio z oknem
app = Application(backend="win32").connect(title_re=".*Form1.*")

# Znalezienie okna aplikacji; możesz potrzebować dostosować ten selektor
main_window = app.window(title_re=".*Form1.*")


for index, row in df.iloc[4:1680].iterrows():
    if row.iloc[2:11].isnull().all():
        break

    sleep(1)
    # Wpisywanie danych z kolumn C-G
    instancja = str(row.iloc[2]) 
    main_window.child_window(auto_id="textBox1").type_keys(instancja, with_spaces=True)
    value_to_type2 = str(row.iloc[3]) 
    main_window.child_window(auto_id="textBox5").type_keys(value_to_type2, with_spaces=True)
    value_to_type3 = str(row.iloc[4]) 
    main_window.child_window(auto_id="textBox3").type_keys(value_to_type3, with_spaces=True)
    value_to_type4 = str(row.iloc[5]) 
    main_window.child_window(auto_id="textBox4").type_keys(value_to_type4, with_spaces=True)
    value_to_type5 = str(row.iloc[6]) 
    main_window.child_window(auto_id="textBox2").type_keys(value_to_type5, with_spaces=True)

    # Kliknięcie pierwszego przycisku
    main_window.child_window(auto_id="button3").click()
    sleep(1)  # Czekaj na przetworzenie

    mapawejsc = main_window.child_window(auto_id="richTextBox2").window_text()

    main_window.child_window(auto_id="button8").click()

    #metaheurystyka

    value_to_type6 = str(row.iloc[7]) 
    main_window.child_window(auto_id="textBox6").type_keys(value_to_type6, with_spaces=True)
    value_to_type7 = str(row.iloc[8]) 
    main_window.child_window(auto_id="textBox10").type_keys(value_to_type7, with_spaces=True)
    value_to_type8 = str(row.iloc[9]) 
    main_window.child_window(auto_id="textBox8").type_keys(value_to_type8, with_spaces=True)
    value_to_type9 = str(row.iloc[10]) 
    main_window.child_window(auto_id="textBox9").type_keys(value_to_type9, with_spaces=True)
    
    # Kliknięcie drugiego przycisku
    main_window.child_window(auto_id="button6").click()
    sleep(1)
    main_window.child_window(auto_id="button9").click()
    sleep(25)  # Czekaj na przetworzenie i zmianę wartości w textbox

    # Odczytanie danych z textbox
    
    mapaCiecWejsc = main_window.child_window(auto_id="richTextBox5").window_text()
    liczbaWejsc = main_window.child_window(auto_id="richTextBox9").window_text()
    mapaWyjsc = main_window.child_window(auto_id="richTextBox6").window_text()
    Liczbawyjsc = main_window.child_window(auto_id="richTextBox9").window_text()
    procentMapa = main_window.child_window(auto_id="richTextBox9").window_text()
    procentMultizbior = main_window.child_window(auto_id="richTextBox11").window_text()
    czas = main_window.child_window(auto_id="richTextBox12").window_text()

    # Zapisanie wyników do DataFrame
    df.iloc[index, 12] = mapawejsc
    df.iloc[index, 13] = mapaCiecWejsc
    df.iloc[index, 14] = liczbaWejsc
    df.iloc[index, 15] = mapaWyjsc
    df.iloc[index, 16] = Liczbawyjsc
    df.iloc[index, 17] = procentMapa
    df.iloc[index, 18] = procentMultizbior
    df.iloc[index, 19] = czas

    sleep(1)
    main_window.child_window(auto_id="button10").click()


# Zapisanie zmodyfikowanego DataFrame do nowego pliku Excel
df.to_excel(excel_path, index=False)

main_window.close()