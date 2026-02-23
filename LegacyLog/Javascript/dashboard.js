function login() {
  document.getElementById("loginPanel").style.display = "none";
  document.getElementById("dashboard").style.display = "block";

  document.getElementById("collection").innerHTML = `
    <li>NES Console — Preservation Complete</li>
    <li>SNES Cartridge Lot — In Progress</li>
  `;
}
