using PropertyChanged;
using SUP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SUP.ViewModels;

[AddINotifyPropertyChangedInterface]

public class CardViewModel
{
    public int Id { get; set; }
    public ICommand ClickCommand { get; set; }
    public bool FaceUp { get; set; }

    public ImageSource Image { get; set; }
    //public Brush Color {  get; set; }

    public CardViewModel(Cards c, Action<CardViewModel> onClick)
    {
        Id = c.Id;
        Image = c.Image;
        //Color = c.Color;
        FaceUp = c.FaceUp;
        ClickCommand = new RelayCommand(_ => onClick(this)); //hela kortet får man info om, inte bara id som tidigare
    }
}
