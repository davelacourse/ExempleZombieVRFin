using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class UISelectionAvatar : MonoBehaviour
{
    public static UISelectionAvatar Instance;

    [SerializeField] private Button _btTeteDroite = default;
    [SerializeField] private Button _btTeteGauche = default;
    [SerializeField] private Button _btBodyDroite = default;
    [SerializeField] private Button _btBodyGauche = default;
    [SerializeField] private Slider _couleurSlider = default;
    [SerializeField] private TMP_InputField _nomAvatar = default;
    
    private ModifierAvatarReseau _modifierAvatarReseau;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Association des boutons du UI pour appeler les m�thodes correspondantes
        _btTeteDroite.onClick.AddListener(() => UpdateIndexTete(_modifierAvatarReseau._teteIndex.Value + 1));
        _btTeteGauche.onClick.AddListener(() => UpdateIndexTete(_modifierAvatarReseau._teteIndex.Value - 1));
        
        _btBodyDroite.onClick.AddListener(() => UpdateIndexBody(_modifierAvatarReseau._bodyIndex.Value + 1));
        _btBodyGauche.onClick.AddListener(() => UpdateIndexBody(_modifierAvatarReseau._bodyIndex.Value - 1));

        _couleurSlider.onValueChanged.AddListener(UpdateValeurCouleur);

        _nomAvatar.onEndEdit.AddListener(UpdateValeurNom);
    }

    // M�thode qui va recevoir l'Avatar cr�e al�atoirement
    public void Initialize(ModifierAvatarReseau modifierAvatarReseau)
    {
        // On l'assigne � la variable de notre avatar courant
        _modifierAvatarReseau = modifierAvatarReseau;

        // On change le texte du champ texte pour le nom sans d�clencher le Addlistener du champ
        _nomAvatar.SetTextWithoutNotify(_modifierAvatarReseau._nomAvatar.Value.ToString());
        _couleurSlider.SetValueWithoutNotify(_modifierAvatarReseau._couleurAvatar.Value);
    }

    // M�thode qui change le mod�le de t�te avec l'index re�u
    public void UpdateIndexTete(int newIndex)
    {
        // Si l'avatar n'existe pas on sort de la m�thode
        if (!_modifierAvatarReseau)
            return;

        // Si l'index d�passe le nombre de morceau dans la liste on le remets � 0
        if(newIndex >= _modifierAvatarReseau.OptionsTete.Length)
        {
            newIndex = 0;
        }

        //Si l'index est plus petit que 0 on le remet � la fin de la liste
        if(newIndex < 0)
        {
            newIndex = _modifierAvatarReseau.OptionsTete.Length - 1;
        }

        _modifierAvatarReseau._teteIndex.Value = newIndex;
    }

    // M�thode qui change le mod�le de body avec l'index re�u
    public void UpdateIndexBody(int newIndex)
    {
        // Si l'avatar n'existe pas on sort de la m�thode
        if (!_modifierAvatarReseau)
            return;

        // Si l'index d�passe le nombre de morceau dans la liste on le remets � 0
        if (newIndex >= _modifierAvatarReseau.OptionsBody.Length)
        {
            newIndex = 0;
        }

        //Si l'index est plus petit que 0 on le remet � la fin de la liste
        if (newIndex < 0)
        {
            newIndex = _modifierAvatarReseau.OptionsBody.Length - 1;
        }

        _modifierAvatarReseau._bodyIndex.Value = newIndex;
    }

    public void UpdateValeurCouleur(float newValue)
    {
        // Si l'avatar n'existe pas on sort de la m�thode
        if (!_modifierAvatarReseau)
            return;

        _modifierAvatarReseau._couleurAvatar.Value = newValue;
    }

    public void UpdateValeurNom(string newValue)
    {
        // Si l'avatar n'existe pas on sort de la m�thode
        if (!_modifierAvatarReseau)
            return;

        _modifierAvatarReseau._nomAvatar.Value = newValue;
    }

}
