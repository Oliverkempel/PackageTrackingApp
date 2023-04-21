# Maltmover Report
## Humlehejsen rapport

For at kompilere projektet kan følgende bruges:
```bash
sudo ./latexdockercmd.sh pdflatex -interaction=nonstopmode main.tex
```
Man skal bruge WSL

Aux filer kan fjernes med.. pas på tho, lortet kan gå i stykker hvis man compiler uden dem her :think:
```bash
rm *.out *.bcf *.aux *.log *.xml *.toc
```

Onelineren bliver 
```bash
sudo ./latexdockercmd.sh pdflatex -interaction=nonstopmode main.tex; rm *.out *.bcf *.aux *.log *.xml *.toc
```

Hvis det er lidt cringe
```bash
sudo pdflatex -shell-escape -interaction=nonstopmode main.tex
```