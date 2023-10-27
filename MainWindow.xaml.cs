using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation; 
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Windows.Media.Animation;
using System.Data.SqlClient;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Collections;
using System.Net.NetworkInformation;

namespace ATM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string account_number;
        string pin_input_final;
        string data;
        string saldo;
        string bank_id;
        string selectedBankAccountNumber_Admin;

        public MainWindow()
        {
            InitializeComponent();

        }



        public void getLatestTransactions()
        {
            string sqlQuery = "SELECT `date`, `amount`, CASE WHEN `storten` = 1 THEN 'true' ELSE 'false' END AS `storten` FROM `transactions` ORDER BY `date` DESC LIMIT 3";
            
            // Create a DataTable to hold the results
            DataTable dataTable = new DataTable();

            // Create a MySqlConnection
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                // Create a MySqlCommand
                using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                {
                    // Open the connection
                    connection.Open();

                    // Create a MySqlDataAdapter to fill the DataTable
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        // Fill the DataTable with the results from the MySQL query
                        adapter.Fill(dataTable);
                    }
                }
            }

            // Bind the DataTable to the DataGrid
            dataGrid.ItemsSource = dataTable.DefaultView;
        }

        public int geld_storten(string bedrag, string datum)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();

                string sql = "INSERT INTO transactions (bank_accounts_bank_accounts_id, amount, storten) VALUES (@value1, @value2, @value3)";
                using (MySqlCommand command = new MySqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@value1", $"{bank_id}");
                    command.Parameters.AddWithValue("@value2", $"{bedrag}");
                    command.Parameters.AddWithValue("@value3", 1);

                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");

                }
                string oud_bedrag = getData($"SELECT balance FROM bank_accounts WHERE bank_accounts_number = '{account_number}'");
                int nieuw_bedrag = int.Parse(bedrag) + int.Parse(oud_bedrag);
                string sql1 = $"UPDATE bank_accounts SET balance = '{nieuw_bedrag}' WHERE bank_accounts_id = '{bank_id}'";
                using (MySqlCommand command = new MySqlCommand(sql1, conn))
                {
                    int rowsAffected1 = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected1}");
                    return rowsAffected1;
                }
            }
        }

        public int aantal_transacties_dag()
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();
                string sql = $"SELECT COUNT(*) FROM transactions WHERE DATE(date) = CURDATE() AND bank_accounts_bank_accounts_id = {bank_id} AND storten = 0";
                using (MySqlCommand command = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                        }
                    }

                }
            }
            return int.Parse(data);
        }

        public int geld_opnemen(string bedrag)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();

                string sql2 = $"SELECT COUNT(*) FROM transactions WHERE DATE(date) = CURDATE() AND bank_accounts_bank_accounts_id = {bank_id} AND storten = 0";
                using (MySqlCommand command = new MySqlCommand(sql2, conn))
                {
                     using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                            if (int.Parse(data) == 3)
                            {
                                
                            }
                        }
                    }

                }
                    string sql = "INSERT INTO transactions (bank_accounts_bank_accounts_id, amount, storten) VALUES (@value1, @value2, @value3)";
                using (MySqlCommand command = new MySqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@value1", $"{bank_id}");
                    command.Parameters.AddWithValue("@value2", $"{bedrag}");
                    command.Parameters.AddWithValue("@value3", 0);

                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                string oud_bedrag = getData($"SELECT balance FROM bank_accounts WHERE bank_accounts_number = '{account_number}'");
                int nieuw_bedrag = int.Parse(oud_bedrag) - int.Parse(bedrag);
                string sql1 = $"UPDATE bank_accounts SET balance = '{nieuw_bedrag}' WHERE bank_accounts_id = '{bank_id}'";
                using (MySqlCommand command = new MySqlCommand(sql1, conn))
                {
                    int rowsAffected1 = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected1}");
                    return rowsAffected1;
                }
            }
        }
        public bool Login(string query)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int rowCount = 0;

                        while (reader.Read())
                        {
                            rowCount++;
                        }

                        if (rowCount == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public string getData(string query)
        {
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    // Execute the query and retrieve the result
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                            //data = reader.GetDecimal(0).ToString();
                        }
                    }
                }
            }
            Console.WriteLine(data);
            return data;
        }


        public static string ComputeSha256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (accountnumber_text.Text == "NLADMIN")
            {
                login.Visibility = Visibility.Hidden;
                admin_login.Visibility = Visibility.Visible;
            }
            else
            {
                account_number = accountnumber_text.Text;
                login.Visibility = Visibility.Hidden;
                enter_pincode.Visibility = Visibility.Visible;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {


        }

        private void enter_pincode_Click(object sender, RoutedEventArgs e)
        {
            pin_input_final = pin_input.Text;

            string pin_input_hashed = ComputeSha256Hash(pin_input_final);

            string query = $"SELECT * FROM `bank_accounts` WHERE bank_accounts_number = '{account_number}' AND pincode = '{pin_input_hashed}';";

            bool hasExactlyOneRow = Login(query);

            if (hasExactlyOneRow)
            {
                Console.WriteLine("successvol ingelogd");
                gebruikersSchermInladen();
                enter_pincode.Visibility = Visibility.Hidden;
                MainMenu.Visibility = Visibility.Visible;

            }
            else
            {
                MessageBox.Show("De ingevoerde pincode is onjuist", "Error");
                Console.WriteLine("Pincode onjuist");
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonText = button.Content.ToString();
                if (pin_input.Text.Length < 4)
                {
                    pin_input.Text += buttonText;
                }
            }
        }

        private void opneemNumber_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonText = button.Content.ToString();
                opneemBedrag.Text += buttonText;
                
            }
        }

        private void gebruikersSchermInladen()
        {
            saldo = getData($"SELECT balance FROM bank_accounts WHERE bank_accounts_number = '{account_number}'");
            bank_id = getData($"SELECT bank_accounts_id FROM bank_accounts WHERE bank_accounts_number = '{account_number}'");
            getData($"SELECT pincode FROM bank_accounts WHERE bank_accounts_number = '{account_number}'");
            welkomGebruiker.Text = $"Welkom gebruiker, {account_number}";
            uwSaldo.Text = $"Uw saldo is: €{saldo}";

        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(pin_input.Text))
            {
                pin_input.Text = pin_input.Text.Substring(0, pin_input.Text.Length - 1);
            }
        }

        private void Opnemen_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Hidden;
            opnemen.Visibility = Visibility.Visible;
        }

        private void Storten_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Hidden;
            storten.Visibility = Visibility.Visible;
        }
        private void opnemenGeld_Click(object sender, RoutedEventArgs e)
        {
            string bedrag = opneemBedrag.Text;
            string epochTimestampAsString = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            Console.WriteLine(aantal_transacties_dag());
            if (aantal_transacties_dag() < 3)
            {
                if (int.Parse(bedrag) < 500)
                {
                    if (int.Parse(bedrag) < int.Parse(getData($"SELECT balance FROM bank_accounts WHERE bank_accounts_number = '{account_number}'")))
                    {
                        if (geld_opnemen(bedrag) == 1)
                        {
                            MessageBox.Show("Je hebt succesvol geld opgenomen", "Success");
                        }
                        else
                        {
                            MessageBox.Show("Er is iets fout gegaan tijdens opnemen", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Je hebt niet genoeg geld op je rekening staan", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Je mag niet meer dan €500 opnemen", "Error");
                }
            }
            else
            {
                MessageBox.Show("Je hebt al 3x geld opgenomen", "Error");
            }

        }

        private void stortenGeld_Click(object sender, RoutedEventArgs e)
        {
            string bedrag = stortenBedrag.Text;
            string epochTimestampAsString = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            if (geld_storten(bedrag, epochTimestampAsString) == 1)
            {
                MessageBox.Show("Je hebt succesvol geld gestoord", "Success");
            }
            else
            {
                MessageBox.Show("Er is iets fout gegaan tijdens storten", "Error");
            }
        }

        private void showTransactions_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Hidden;
            getLatestTransactions();
            laatsteTransactiesGrid.Visibility = Visibility.Visible;

            // Number of rectangular blocks to create

        }

        private void TerugOpnemen_Click(object sender, RoutedEventArgs e)
        {
            gebruikersSchermInladen();
            opnemen.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Visible;
        }

        private void TerugStorten_Click(object sender, RoutedEventArgs e)
        {
            gebruikersSchermInladen();
            storten.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Visible;
        }

        private void Delete_opneemNumber_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(opneemBedrag.Text))
            {
                opneemBedrag.Text = opneemBedrag.Text.Substring(0, opneemBedrag.Text.Length - 1);
            }
        }

        private void admin_NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonText = button.Content.ToString();
                if (pin_admin_input.Text.Length < 4)
                {
                    pin_admin_input.Text += buttonText;
                }
            }
        }

        private void admin_Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(pin_admin_input.Text))
            {
                pin_admin_input.Text = pin_admin_input.Text.Substring(0, pin_admin_input.Text.Length - 1);
            }
        }

        private void admin_enter_pincode_Click(object sender, RoutedEventArgs e)
        {
            if (pin_admin_input.Text == "1234")
            {
                admin_login.Visibility = Visibility.Hidden;
                laat_alle_rekeninge_zien();
                admin_Rekeningen.Visibility = Visibility.Visible;

            }
            else
            {
                MessageBox.Show("De ingevoerde pincode is onjuist", "Error");
            }
        }

        private void changeBalance(string rekeningnummer, string balance)
        {
            string query = $"UPDATE bank_accounts SET balance = {balance} WHERE bank_accounts_number = '{rekeningnummer}'";
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    // Execute the query and retrieve the result
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                            //data = reader.GetDecimal(0).ToString();
                        }
                    }
                }
            }
            }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Rekeningen_admin.SelectedItem != null)
            {
                // Cast the selected item to a string (assuming the items are strings)
                string selectedValue = Rekeningen_admin.SelectedItem.ToString();

                string[] parts = selectedValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string part in parts)
                {
                    // Trim any leading/trailing whitespace
                    string trimmedPart = part.Trim();

                    // Check if the part starts with "RekeningNummer" or "Saldo"
                    if (trimmedPart.StartsWith("RekeningNummer: "))
                    {
                        // Extract the RekeningNummer value
                        selectedBankAccountNumber_Admin = trimmedPart.Substring("RekeningNummer: ".Length);


                        // Log the RekeningNummer value
                        SelectedBankLabel.Content = selectedBankAccountNumber_Admin;
                    }
                    else
                    {
                        // Handle other parts if needed
                    }
                }
            }
            else
            {
            }
        }

        private void laat_alle_rekeninge_zien()
        {
            string sqlQuery = "SELECT * FROM `bank_accounts`";

            // Create a MySqlConnection
            using (MySqlConnection connection = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                // Create a MySqlCommand
                using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                {
                    // Open the connection
                    connection.Open();

                    // Create a MySqlDataReader to read data from the database
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Clear the existing items in the ListBox
                        Rekeningen_admin.Items.Clear();

                        // Loop through the query results and add items to the ListBox
                        while (reader.Read())
                        {
                            // Create a string to represent each row of data
                            string row = $"RekeningNummer: {reader["bank_accounts_number"]} \nSaldo: {reader["balance"]} \n";

                            // Add the row to the ListBox
                            Rekeningen_admin.Items.Add(row);
                        }
                    }
                }
            }
        }

        private void updateSaldo_Admin_Click(object sender, RoutedEventArgs e)
        {
            admin_Rekeningen.Visibility = Visibility.Hidden;
            wijzigSaldo_Admin_Grid.Visibility = Visibility.Visible;
            string saldo_admin = getData($"SELECT balance FROM bank_accounts WHERE bank_accounts_number = '{selectedBankAccountNumber_Admin}'");
            currentBalance_admin.Text = saldo_admin;

        }

        private void rekeningWissen()
        {
            string sqlQuery = $"DELETE FROM bank_accounts WHERE bank_accounts_number = '{selectedBankAccountNumber_Admin}'";
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(sqlQuery, conn))
                {
                    // Execute the query and retrieve the result
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                            //data = reader.GetDecimal(0).ToString();
                        }
                    }
                }
            }
        }

        private void updateCurrentSaldo_Admin_Click(object sender, RoutedEventArgs e)
        {
            string new_balance = currentBalance_admin.Text;
            changeBalance(selectedBankAccountNumber_Admin, new_balance);
            laat_alle_rekeninge_zien();
            wijzigSaldo_Admin_Grid.Visibility = Visibility.Hidden;
            admin_Rekeningen.Visibility= Visibility.Visible;

        }

        private void deleteAccount_Admin_Click(object sender, RoutedEventArgs e)
        {
            rekeningWissen();
            laat_alle_rekeninge_zien();
        }

        private void changeCurrentPin_Admin_Click(object sender, RoutedEventArgs e)
        {
            string new_pincode = wijzigPincode_admin.Text;
            string new_pin_input_hashed = ComputeSha256Hash(new_pincode);
            changePin(selectedBankAccountNumber_Admin, new_pin_input_hashed);
            laat_alle_rekeninge_zien();
            wijzigpinCode_Admin_Grid.Visibility= Visibility.Hidden;
            admin_Rekeningen.Visibility = Visibility.Visible;

        }

        private void changePin_Admin_Click(object sender, RoutedEventArgs e)
        {
            admin_Rekeningen.Visibility = Visibility.Hidden;
            wijzigpinCode_Admin_Grid.Visibility = Visibility.Visible;
        }

        private void changePin(string rekeningnummer, string pin)
        {
            string query = $"UPDATE bank_accounts SET pincode = '{pin}' WHERE bank_accounts_number = '{rekeningnummer}'";
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    // Execute the query and retrieve the result
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                            //data = reader.GetDecimal(0).ToString();
                        }
                    }
                }
            }
        }
        
        private void rekeningToevoegen(string rekeningnummer, string pincode, string saldo)
        {
            string query = $"INSERT INTO bank_accounts (bank_accounts_number, pincode, balance, ...) VALUES ({rekeningnummer}, {pincode}, {saldo});";
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    // Execute the query and retrieve the result
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                            //data = reader.GetDecimal(0).ToString();
                        }
                    }
                }
            }
        }


        private void voegRekening_Admin_Click(object sender, RoutedEventArgs e)
        {
            admin_Rekeningen.Visibility = Visibility.Hidden;
            VoegRekeningToe_Admin_Grid.Visibility = Visibility.Visible;
        }

        private void voegAccountToe(object sender, RoutedEventArgs e)
        {
            string rekeningNummer = rekeningNummer_Toevoeg.Text;
            string saldo = saldo_Toevoeg.Text;
            string pincode = pinCode_Toevoeg.Text;
            string pingcode_hashed = ComputeSha256Hash(pincode);
            string query = $"INSERT INTO `bank_accounts`(`bank_accounts_number`, `pincode`, `balance`) VALUES ('{rekeningNummer}','{pingcode_hashed}','{saldo}')";
            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Uid=root;Pwd=;database=mydb"))
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    // Execute the query and retrieve the result
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetString(0);
                            //data = reader.GetDecimal(0).ToString();
                        }
                    }
                }
            }
            VoegRekeningToe_Admin_Grid.Visibility = Visibility.Hidden;
            admin_Rekeningen.Visibility= Visibility.Visible;
            laat_alle_rekeninge_zien();
        }
    }
}
