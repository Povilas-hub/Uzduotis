using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;


class Naudotojas
{
    public string Vardas { get; protected set; }
    public string Slaptazodis { get; protected set; }

    public Naudotojas(string vardas, string slaptazodis)
    {
        Vardas = vardas;
        Slaptazodis = slaptazodis;
    }
}

class Administratorius : Naudotojas
{
    public Administratorius(string vardas, string slaptazodis) : base(vardas, slaptazodis)
    {
    }
}

class Vadybininkas : Naudotojas
{
    public Vadybininkas(string vardas, string slaptazodis) : base(vardas, slaptazodis)
    {
    }
}

class Gyventojas : Naudotojas
{
    public string Bendrija { get; private set; }

    public Gyventojas(string vardas, string slaptazodis, string bendrija) : base(vardas, slaptazodis)
    {
        Bendrija = bendrija;
    }
}

class Bendrija
{
    public string Pavadinimas { get; private set; }
    public decimal ElektrosKaina { get; set; }
    public decimal VandensKaina { get; set; }
    public decimal SildymoKaina { get; set; }

    public Bendrija(string pavadinimas, decimal elektrosKaina, decimal vandensKaina, decimal sildymoKaina)
    {
        Pavadinimas = pavadinimas;
        ElektrosKaina = elektrosKaina;
        VandensKaina = vandensKaina;
        SildymoKaina = sildymoKaina;
    }
}

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "server=localhost;uid=root;pwd=root;database=duomenys";
        MySqlConnection connection = new MySqlConnection(connectionString);

        List<Naudotojas> naudotojai = new List<Naudotojas>();
        List<Bendrija> bendrijos = new List<Bendrija>();

        using (connection)
        {
            connection.Open();

            string vartotojaiQuery = "SELECT vardas, slaptazodis, bendrija, vartotojo_tipas FROM Vartotojai";
            using (MySqlCommand vartotojaiCommand = new MySqlCommand(vartotojaiQuery, connection))
            using (MySqlDataReader vartotojaiReader = vartotojaiCommand.ExecuteReader())
            {
                while (vartotojaiReader.Read())
                {
                    string vardas = vartotojaiReader.GetString("vardas");
                    string slaptazodis = vartotojaiReader.GetString("slaptazodis");
                    string bendrija = vartotojaiReader.GetString("bendrija");
                    string vartotojoTipas = vartotojaiReader.GetString("vartotojo_tipas");

                    switch (vartotojoTipas)
                    {
                        case "adminas":
                            Administratorius admin = new Administratorius(vardas, slaptazodis);
                            naudotojai.Add(admin);
                            break;
                        case "vadybininkas":
                            Vadybininkas vadybininkas = new Vadybininkas(vardas, slaptazodis);
                            naudotojai.Add(vadybininkas);
                            break;
                        case "gyventojas":
                            Gyventojas gyventojas = new Gyventojas(vardas, slaptazodis, bendrija);
                            naudotojai.Add(gyventojas);
                            break;
                    }
                }
            }

            string bendrijosQuery = "SELECT pavadinimas, elektros_kaina, vandens_kaina, sildymo_kaina FROM Bendrija";
            using (MySqlCommand bendrijosCommand = new MySqlCommand(bendrijosQuery, connection))
            using (MySqlDataReader bendrijosReader = bendrijosCommand.ExecuteReader())
            {
                while (bendrijosReader.Read())
                {
                    string pavadinimas = bendrijosReader.GetString("pavadinimas");
                    decimal elektrosKaina = bendrijosReader.GetDecimal("elektros_kaina");
                    decimal vandensKaina = bendrijosReader.GetDecimal("vandens_kaina");
                    decimal sildymoKaina = bendrijosReader.GetDecimal("sildymo_kaina");

                    Bendrija bendrija = new Bendrija(pavadinimas, elektrosKaina, vandensKaina, sildymoKaina);
                    bendrijos.Add(bendrija);
                }
            }

            connection.Close();
        }

        Console.WriteLine("Iveskite varda");
        string username = Console.ReadLine();

        Console.WriteLine("Iveskite slaptazodi");
        string password = Console.ReadLine();

        Naudotojas currentUser = naudotojai.Find(n => n.Vardas == username && n.Slaptazodis == password);

        if (currentUser != null)
        {
            if (currentUser is Gyventojas gyventojas)
            {
                Console.WriteLine("Vartotojas " + gyventojas.Vardas);
                Console.WriteLine("Bendrija " + gyventojas.Bendrija);
                Console.WriteLine();

                Bendrija selectedBendrija = bendrijos.Find(b => b.Pavadinimas == gyventojas.Bendrija);
                if (selectedBendrija != null)
                {
                    Console.WriteLine("Bendrija " + selectedBendrija.Pavadinimas);
                    Console.WriteLine("Elektros kaina: " + selectedBendrija.ElektrosKaina);
                    Console.WriteLine("Vandens kaina: " + selectedBendrija.VandensKaina);
                    Console.WriteLine("Sildymo kaina: " + selectedBendrija.SildymoKaina);
                }
                else
                {
                    Console.WriteLine("Bendrija nesurasta");
                }
            }
            else if (currentUser is Vadybininkas)
    {
        Console.WriteLine("Vartotojas " + currentUser.Vardas);
        Console.WriteLine();

        Console.WriteLine("Iveskite bendrija");
        string communityName = Console.ReadLine();

        Bendrija selectedBendrija = bendrijos.Find(b => b.Pavadinimas == communityName);
        if (selectedBendrija != null)
        {
            Console.WriteLine("Bendrija: " + selectedBendrija.Pavadinimas);
            Console.WriteLine("Elektros kaina:  " + selectedBendrija.ElektrosKaina);
            Console.WriteLine("Vandens kaina: " + selectedBendrija.VandensKaina);
            Console.WriteLine("Sildymo kaina: " + selectedBendrija.SildymoKaina);

            Console.WriteLine("Iveskite nauja elektros kaina :");
            decimal newElektrosKaina = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Iveskite nauja vandens kaina:");
            decimal newVandensKaina = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Iveskite nauja sildymo kaina:");
            decimal newSildymoKaina = decimal.Parse(Console.ReadLine());

            selectedBendrija.ElektrosKaina = newElektrosKaina;
            selectedBendrija.VandensKaina = newVandensKaina;
            selectedBendrija.SildymoKaina = newSildymoKaina;

            using (connection)
            {
                connection.Open(); 

                string updateQuery = "UPDATE Bendrija SET elektros_kaina = @elektrosKaina, vandens_kaina = @vandensKaina, sildymo_kaina = @sildymoKaina WHERE pavadinimas = @pavadinimas";
                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@elektrosKaina", newElektrosKaina);
                    updateCommand.Parameters.AddWithValue("@vandensKaina", newVandensKaina);
                    updateCommand.Parameters.AddWithValue("@sildymoKaina", newSildymoKaina);
                    updateCommand.Parameters.AddWithValue("@pavadinimas", selectedBendrija.Pavadinimas);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    Console.WriteLine("Atnaujinta " + rowsAffected + " eilutes.");
                }

                connection.Close(); 
            }
        }
        else
        {
            Console.WriteLine("Bendrija nesurasta.");
        }
            }
            else if (currentUser is Administratorius)
            {
                using (connection)
                {
                    connection.Open();
                Console.WriteLine("Vartotojas: " + currentUser.Vardas);
                Console.WriteLine();

                Console.WriteLine("Iveskite bendrija:");
                string communityName = Console.ReadLine();

                Bendrija selectedBendrija = bendrijos.Find(b => b.Pavadinimas == communityName);
                if (selectedBendrija != null)
                {
                    Console.WriteLine("Bendrija: " + selectedBendrija.Pavadinimas);
                    Console.WriteLine("Elektros kaina  " + selectedBendrija.ElektrosKaina);
                    Console.WriteLine("Vandens kaina: " + selectedBendrija.VandensKaina);
                    Console.WriteLine("Sildymo kaina: " + selectedBendrija.SildymoKaina);
                    Console.WriteLine();

                    Console.WriteLine("Iveskite 'create', kad sukurtumete nauja bendrija arba iveskite 'delete', kad istrintumete bendrija:");
                    string action = Console.ReadLine();

                    if (action.ToLower() == "create")
                    {
                        Console.WriteLine("Iveskite nauja bendrijos varda:");
                        string newCommunityName = Console.ReadLine();
                        Console.WriteLine("Iveskite nauja elektros kaina:");
                        decimal newElektrosKaina = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Iveskite nauja vandens kaina:");
                        decimal newVandensKaina = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Iveskite nauja sildymo kaina:");
                        decimal newSildymoKaina = decimal.Parse(Console.ReadLine());

                        Bendrija newBendrija = new Bendrija(newCommunityName, newElektrosKaina, newVandensKaina, newSildymoKaina);
                        bendrijos.Add(newBendrija);

                        string insertQuery = "INSERT INTO Bendrija (pavadinimas, elektros_kaina, vandens_kaina, sildymo_kaina) VALUES (@pavadinimas, @elektrosKaina, @vandensKaina, @sildymoKaina)";
                        using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@pavadinimas", newCommunityName);
                            insertCommand.Parameters.AddWithValue("@elektrosKaina", newElektrosKaina);
                            insertCommand.Parameters.AddWithValue("@vandensKaina", newVandensKaina);
                            insertCommand.Parameters.AddWithValue("@sildymoKaina", newSildymoKaina);

                            int rowsAffected = insertCommand.ExecuteNonQuery();
                            Console.WriteLine("Atnaujinta " + rowsAffected + " eilutes.");
                        }
                    }
                    else if (action.ToLower() == "delete")
                    {
                        bendrijos.Remove(selectedBendrija);

                        string deleteQuery = "DELETE FROM Bendrija WHERE pavadinimas = @pavadinimas";
                        using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@pavadinimas", selectedBendrija.Pavadinimas);

                            int rowsAffected = deleteCommand.ExecuteNonQuery();
                            Console.WriteLine("Istrinta " + rowsAffected + " eilutes.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Klaida.");
                    }
                }
                else
                {
                    Console.WriteLine("Bendrija nerasta.");
                }
                connection.Close();
                }
            }
        }
        else
        {
            Console.WriteLine("Neteisingas vartotojas arba slaptazodis.");
        }
    }
}
