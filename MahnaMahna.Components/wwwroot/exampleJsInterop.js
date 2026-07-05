//JavaScript modul igény szerint (on-demand) töltõdik be, bármennyi függvényt exportálm és szükség esetén más JS modulokat is importál

export function showPrompt(message) {
  return prompt(message, 'Type anything here');
}
