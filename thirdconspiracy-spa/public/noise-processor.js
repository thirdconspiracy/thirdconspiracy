/**
 * Noise generator for the Focus one (AudioWorklet - runs on the audio thread)
 * noiseType: 0 = white, 1 = pink (Paul Kellet's filter), 2 = brown (integrated white)
 * Each type is scaled to the same RMS (~0.15, about -16.5 dBFS) so they sit at a similar
 * loudness, below typical mastered music, leaving headroom when mixed with Focus music
 */
const WHITE_GAIN = 0.26; // raw RMS ~.577
const PINK_GAIN = 0.086; // raw RMS ~1.76 before scaling
const BROWN_GAIN = 2.6; // raw RMS ~0.0575

class NoiseProcessor extends AudioWorkletProcessor {
  static parameterDescriptors = [
    { name: 'noiseType', defaultValue: 0, minValue: 0, maxValue: 2, automationRate: 'k-rate' },
  ];

  // Pink noise filter state
  b = [0, 0, 0, 0, 0, 0, 0];
  lastOut = 0;

  process(_inputs, outputs, parameters) {
    const noiseType = Math.round(parameters.noiseType[0]);
    const next = noiseType === 1 ? () => this.pink() : noiseType === 2 ? () => this.brown() : () => (Math.random() * 2 -1) * WHITE_GAIN;

    // Mono noise copied to every output channel
    const [first, ...rest] = outputs[0];
    for (let i = 0; i < first.length; i++) first[i] = next();
    for (const channel of rest) channel.set(first);
    return true;
  }

  pink() {
    const white = Math.random() * 2 - 1;
    const b = this.b;
    b[0] = 0.99886 * b[0] + white * 0.0555179;
    b[1] = 0.99332 * b[1] + white * 0.0750759;
    b[2] = 0.969 * b[2] + white * 0.153852;
    b[3] = 0.8665 * b[3] + white * 0.3104856;
    b[4] = 0.55 * b[4] + white * 0.5329522;
    b[5] = -0.7616 * b[5] - white * 0.016898;
    const out = b[0] + b[1] + b[2] + b[3] + b[4] + b[5] + b[6] + white * 0.5362;
    // b[6] is a one-sample delay, so it's updated after being summed.
    b[6] = white * 0.115926;
    return out * PINK_GAIN;
  }

  brown() {
    const white = Math.random() * 2 - 1;
    this.lastOut = (this.lastOut + 0.02 * white) / 1.02;
    return this.lastOut * BROWN_GAIN;
  }
}

registerProcessor('noise-processor', NoiseProcessor);
