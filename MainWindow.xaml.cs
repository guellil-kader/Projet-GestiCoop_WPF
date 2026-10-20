using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace GestiCoop;

public class OffreStage
{
    public string Entreprise { get; set; } = "";
    public string Titre { get; set; } = "";
    public string Programme { get; set; } = "";
    public string TypeStage { get; set; } = "";
    public string Lieu { get; set; } = "";
    public decimal Salaire { get; set; }
    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }
    public DateTime Limite { get; set; }
    public int Postes { get; set; }
    public string Description { get; set; } = "";
    public string Statut { get; set; } = "Ouverte";
}

public partial class MainWindow : Window
{
    private readonly ObservableCollection<OffreStage> _offres = new();
    private OffreStage? _offreEnModification;

    public MainWindow()
    {
        InitializeComponent();
        GridOffres.ItemsSource = _offres;

        // Quelques données de démonstration, modifiables et supprimables.
        _offres.Add(new OffreStage { Entreprise = "TechNova", Titre = "Développeur junior", Programme = "Développement logiciel", TypeStage = "Stage coopératif", Lieu = "Ottawa, ON", Salaire = 22, Debut = DateTime.Today.AddDays(45), Fin = DateTime.Today.AddDays(135), Limite = DateTime.Today.AddDays(25), Postes = 2, Description = "Participer au développement d'applications internes." });
        _offres.Add(new OffreStage { Entreprise = "Groupe Atlas", Titre = "Technicien informatique", Programme = "Techniques informatiques", TypeStage = "Stage d'été", Lieu = "Gatineau, QC", Salaire = 20, Debut = DateTime.Today.AddDays(60), Fin = DateTime.Today.AddDays(120), Limite = DateTime.Today.AddDays(35), Postes = 1, Description = "Assister l'équipe de soutien technique." });
        _offres.Add(new OffreStage { Entreprise = "Solutions Boréal", Titre = "Analyste de données", Programme = "Analyse de données", TypeStage = "Stage rémunéré", Lieu = "Montréal, QC", Salaire = 24, Debut = DateTime.Today.AddDays(50), Fin = DateTime.Today.AddDays(140), Limite = DateTime.Today.AddDays(30), Postes = 1, Description = "Nettoyage et analyse de données." });
        RefreshList();
    }

    private string SelectedText(ComboBox combo)
    {
        if (combo.SelectedItem is ComboBoxItem item)
            return item.Content?.ToString() ?? "";
        return combo.Text.Trim();
    }

    private void Publish_Click(object sender, RoutedEventArgs e)
    {
        string entreprise = SelectedText(CboEntreprise);
        string programme = SelectedText(CboProgramme);
        string type = SelectedText(CboType);
        string titre = TxtTitre.Text.Trim();
        string lieu = TxtLieu.Text.Trim();
        string description = TxtDescription.Text.Trim();

        if (string.IsNullOrWhiteSpace(entreprise) || entreprise == "Sélectionner une entreprise" ||
            string.IsNullOrWhiteSpace(programme) || programme == "Sélectionner un programme" ||
            string.IsNullOrWhiteSpace(titre) || string.IsNullOrWhiteSpace(lieu) ||
            string.IsNullOrWhiteSpace(description) || DateDebut.SelectedDate is null ||
            DateFin.SelectedDate is null || DateLimite.SelectedDate is null)
        {
            LblMessage.Text = "Veuillez remplir tous les champs obligatoires (*) et choisir les dates.";
            LblMessage.Foreground = System.Windows.Media.Brushes.Firebrick;
            return;
        }

        if (DateFin.SelectedDate.Value < DateDebut.SelectedDate.Value)
        {
            LblMessage.Text = "La date de fin doit être après la date de début.";
            LblMessage.Foreground = System.Windows.Media.Brushes.Firebrick;
            return;
        }

        if (DateLimite.SelectedDate.Value > DateDebut.SelectedDate.Value)
        {
            LblMessage.Text = "La date limite doit être au plus tard la date de début.";
            LblMessage.Foreground = System.Windows.Media.Brushes.Firebrick;
            return;
        }

        if (!decimal.TryParse(TxtSalaire.Text.Trim(), out decimal salaire) || salaire < 0)
        {
            LblMessage.Text = "Le salaire doit être un nombre positif ou égal à zéro.";
            LblMessage.Foreground = System.Windows.Media.Brushes.Firebrick;
            return;
        }

        if (!int.TryParse(TxtPostes.Text.Trim(), out int postes) || postes < 1)
        {
            LblMessage.Text = "Le nombre de postes doit être un entier supérieur à zéro.";
            LblMessage.Foreground = System.Windows.Media.Brushes.Firebrick;
            return;
        }

        if (_offreEnModification is null)
        {
            _offres.Add(new OffreStage
            {
                Entreprise = entreprise, Programme = programme, TypeStage = type, Titre = titre,
                Lieu = lieu, Salaire = salaire, Debut = DateDebut.SelectedDate.Value,
                Fin = DateFin.SelectedDate.Value, Limite = DateLimite.SelectedDate.Value,
                Postes = postes, Description = description, Statut = "Ouverte"
            });
            LblMessage.Text = "Offre publiée avec succès.";
        }
        else
        {
            _offreEnModification.Entreprise = entreprise;
            _offreEnModification.Programme = programme;
            _offreEnModification.TypeStage = type;
            _offreEnModification.Titre = titre;
            _offreEnModification.Lieu = lieu;
            _offreEnModification.Salaire = salaire;
            _offreEnModification.Debut = DateDebut.SelectedDate.Value;
            _offreEnModification.Fin = DateFin.SelectedDate.Value;
            _offreEnModification.Limite = DateLimite.SelectedDate.Value;
            _offreEnModification.Postes = postes;
            _offreEnModification.Description = description;
            _offreEnModification = null;
            LblMessage.Text = "Offre modifiée avec succès.";
        }

        LblMessage.Foreground = System.Windows.Media.Brushes.SeaGreen;
        RefreshList();
        ResetForm(false);
    }

    private void Reset_Click(object sender, RoutedEventArgs e) => ResetForm(true);

    private void ResetForm(bool showMessage)
    {
        _offreEnModification = null;
        CboEntreprise.SelectedIndex = -1;
        CboEntreprise.Text = "";
        CboProgramme.SelectedIndex = -1;
        CboProgramme.Text = "";
        CboType.SelectedIndex = 0;
        TxtTitre.Clear();
        TxtLieu.Clear();
        TxtSalaire.Text = "20.00";
        DateDebut.SelectedDate = null;
        DateFin.SelectedDate = null;
        DateLimite.SelectedDate = null;
        TxtPostes.Text = "1";
        TxtDescription.Clear();
        if (showMessage)
        {
            LblMessage.Text = "Formulaire réinitialisé.";
            LblMessage.Foreground = System.Windows.Media.Brushes.Gray;
        }
    }

    private void RefreshList()
    {
        string search = TxtRecherche?.Text?.Trim() ?? "";
        string filter = CboFiltre?.SelectedItem is ComboBoxItem item ? item.Content?.ToString() ?? "Tous les statuts" : "Tous les statuts";
        var results = _offres.Where(o =>
            (string.IsNullOrWhiteSpace(search) ||
             o.Entreprise.Contains(search, StringComparison.OrdinalIgnoreCase) ||
             o.Titre.Contains(search, StringComparison.OrdinalIgnoreCase) ||
             o.Programme.Contains(search, StringComparison.OrdinalIgnoreCase) ||
             o.Lieu.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
            (filter == "Tous les statuts" || o.Statut == filter)).ToList();
        GridOffres.ItemsSource = results;
        LblNombre.Text = $"Offres affichées : {results.Count}   |   Total : {_offres.Count}";
    }

    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (GridOffres is not null) RefreshList();
    }

    private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridOffres is not null) RefreshList();
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not OffreStage offre) return;
        _offreEnModification = offre;
        CboEntreprise.Text = offre.Entreprise;
        CboProgramme.Text = offre.Programme;
        CboType.SelectedIndex = Math.Max(0, CboType.Items.Cast<ComboBoxItem>().ToList().FindIndex(i => i.Content?.ToString() == offre.TypeStage));
        TxtTitre.Text = offre.Titre;
        TxtLieu.Text = offre.Lieu;
        TxtSalaire.Text = offre.Salaire.ToString("0.00");
        DateDebut.SelectedDate = offre.Debut;
        DateFin.SelectedDate = offre.Fin;
        DateLimite.SelectedDate = offre.Limite;
        TxtPostes.Text = offre.Postes.ToString();
        TxtDescription.Text = offre.Description;
        LblMessage.Text = "Modification en cours : changez les champs puis cliquez sur Publier l'offre.";
        LblMessage.Foreground = System.Windows.Media.Brushes.DarkOrange;
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not OffreStage offre) return;
        var result = MessageBox.Show($"Supprimer l'offre « {offre.Titre} » de {offre.Entreprise} ?",
            "Confirmer la suppression", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _offres.Remove(offre);
            RefreshList();
            LblMessage.Text = "Offre supprimée.";
            LblMessage.Foreground = System.Windows.Media.Brushes.SeaGreen;
        }
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is not string section || section == "offers") return;
        string label = (sender as Button)?.Content?.ToString() ?? section;
        MessageBox.Show($"La section « {label} » pourra être développée dans une prochaine étape.\\n\\nLa section Offres de stage est fonctionnelle.",
            "GestiCoop", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
