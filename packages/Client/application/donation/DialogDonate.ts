import { component, css, DialogComponent, html } from '@3mo/model'
import { Campaign } from 'sdk'

@component('solid-dialog-donate')
export class DialogDonate extends DialogComponent<{ readonly campaign: Campaign }> {
	static override get styles() {
		return css`
			img {
				height: 40px;
			}
		`
	}

	protected override get template() {
		return html`
			<mo-dialog heading='Donate'>
				<mo-flex>
					<mo-list>
						${this.parameters.campaign.donationChannels.map(donationChannel => html`
							<mo-list-item>
								<img slot='graphic' src=${donationChannel.logoSource} />
								${donationChannel.paymentMethodIdentifier}
							</mo-list-item>
						`)}
					</mo-list>
				</mo-flex>
			</mo-dialog>
		`
	}
}