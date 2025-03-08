# Block Collapse Puzzle Game - Good Level Up Case Study  
**A Unity-based puzzle game featuring dynamic grid management, smart deadlock resolution, and performance optimizations.**  
*Developed for the Good Level Up Development Summer Internship Case Study.*

---

## 🎮 Features  
- **Collapse/Blast Mechanic**  
  - Destroy groups of 2+ same-colored blocks.  
  - Fill empty spaces with falling blocks from above.  

- **Dynamic Group Icons**  
  - Icons change based on group size thresholds (A, B, C).  
  - Configure thresholds via UI (e.g., A=4, B=6, C=8).  

- **Smart Deadlock Resolution**  
  - Detects grid deadlocks and resolves them by intelligently swapping blocks.  

- **Flexible Grid & Color System**  
  - Adjust grid size (2-10 rows, 2-12 columns).  
  - Set active colors (1-6) with unique icons.  

- **Performance Optimizations**  
  - Object pooling for efficient block reuse.  
  - Smooth animations with easing functions.  

---

## 🛠️ Requirements  
- Unity 2021.3+ (LTS recommended).  
- TextMeshPro (included in Unity Package Manager).  

---

## 🚀 Installation  
1. **Clone the Repository**  
   ```bash
   git clone https://github.com/your-username/block-collapse-game.git
Open in Unity

Launch Unity Hub.

Add the project folder and open MainScene.unity.

Configure Settings (Optional)

Modify blockPrefabs in GameController to add custom block icons.

🕹️ How to Play
https://togiss.itch.io/casegame
![image](https://github.com/user-attachments/assets/7dabbdf6-d3a6-4a61-8957-b3d1dd4c78be)

Set Parameters via UI

Adjust rows (M), columns (N), colors (K), and thresholds (A/B/C).

Click Apply Changes to update the grid.

Gameplay

Click/Tap blocks to form groups of 2+ same-colored blocks.

Larger groups (>A, >B, >C) display special icons.

Watch blocks fall and refill the grid automatically.

Deadlock Handling

If no moves are possible, the game auto-resolves the deadlock.

⚙️ Configuration
Parameter	Description	Valid Range
M	Rows	2-10
N	Columns	2-12
K	Colors	1-6
A/B/C	Group Icons	Custom
🤝 Contributing
Report Issues

Use GitHub Issues for bugs or suggestions.

Submit Improvements

Fork the repo and open a Pull Request.

Ensure code follows C# best practices.

📜 License
MIT License. See LICENSE for details.

Inspired by Toon Blast & Pet Rescue Saga. Built with Unity and C#.
