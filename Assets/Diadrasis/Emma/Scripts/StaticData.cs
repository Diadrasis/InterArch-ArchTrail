using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData
{
    public static bool isInternetOn;
    public static bool isSendingSampleImage;

    public static string urlForth = ":8000/upload";// "http://139.91.185.53:8000/upload";
    public static string urlRecommendations = ":6666/api/v1.0/recommendations";
    public static string urlPreferences = ":6666/api/v1.0/preferences";
    public static string urlDemographics = "http://diadrasis.net/MuseLearn_Mobile/postData.php?";
    public static string urlInfoResult = "http://ml.culture.upatras.gr/concept.php?name=58&familiarity=1";

    public static string urlVisitorCreate = ":7777/api/profiles/v1.0/create";
    public static string urlVisitorRead = ":7777/api/profiles/v1.0/read";

    public static string urlMuseumLocal = "192.168.1.250";
    public static string urlDiadrasisExternal = "http://muselearn.ddns.net";
    public static string urlServer = "192.168.1.250";

    public static bool isAnonymous = false;
    public static bool isNewVisitor = false;
    public static string visitorData;
    public static string visitorId, visitorPass, visitorFamiliarity;

    public static bool isDevOptionUnlocked;

    public static string newVisitorMessage(string id, string pass)
    {
        return "Παρακαλώ αποθηκεύστε κάπου τα παρακάτω στοιχεία προκειμένου" +
               " να τα χρησιμοποιήσετε κατά την είσοδο σας στην εφαρμογή." + "\n\n" +
               "Visitor ID = " + id + "\n" +
               "Visitor Password = " + pass;
    }

    public static string visitorWelcomeLogin()
    {
        return "Καλωσήρθατε " + visitorId + "\n\n" + "Επιλέξτε να διαρμοφώσετε τα στοιχεία σας πατώντας το κουμπί 'Επεξεργασία'" +
               " ή συνεχίστε στην λειτουργία της ξενάγησης.";
    }

    public static string emptyInputs()
    {
        return "Πρέπει να συμπληρώσετε όλα τα πεδία!";
    }

    public static string errorInputs()
    {
        return "Τα στοιχεία που δώσατε είναι λάθος!\n\nΔοκιμάστε ξανά.";
    }

    public static string dataUpdated()
    {
        return "Τα στοιχεία σας ενημερώθηκαν.";
    }

    public static string errorOnPostData()
    {
        if (isNewVisitor) return "Ο διακομιστής δεν ανταποκρίνεται.\nΠαρακαλώ δοκιμάστε ξανά";

        return "Τα στοιχεία σας δεν ενημερώθηκαν.\nΠαρακαλώ δοκιμάστε ξανά";
    }

}
